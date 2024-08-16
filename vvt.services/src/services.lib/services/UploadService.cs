using vvt.common.lib.dtos;
using vvt.common.lib.models;
using vvt.common.lib.enums;
using vvt.services.lib.models;
using System.Globalization;
using CsvHelper;
using RepoModels = vvt.services.repositories.lib.models;
using vvt.services.repositories.lib;

namespace vvt.services.lib;

public class UploadService : IUploadService
{
    private readonly IFileUploadRepository _fileUploadRepository;
    private readonly ICompanyService _companyService;
    private readonly IEmployeeService _employeeService;

    public UploadService(IFileUploadRepository fileUploadRepository,
                         ICompanyService companyService,
                         IEmployeeService employeeService)
    {
        _fileUploadRepository = fileUploadRepository;
        _companyService = companyService;
        _employeeService = employeeService;
    }

    private IEnumerable<CsvUploadRecord>? ParseCsvFile(RawUploadDto rawUpload)
    {
        // Add exception handling
        IEnumerable<CsvUploadRecord> records;
        using (var reader = new StringReader(rawUpload.RawLine))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            if(!ValidateCSVHeaders(csv))
            {
                return null;
            }
            records = csv.GetRecords<CsvUploadRecord>().ToList();
        }

        return records;
    }

    private bool ValidateCSVHeaders(CsvReader csv)
    {
        // move to external configuration
        var headers = new List<string>() { "CompanyId", "CompanyCode", "CompanyDescription", "EmployeeNumber", "EmployeeFirstName", "EmployeeLastName", "EmployeeEmail", "EmployeeDepartment", "HireDate", "ManagerEmployeeNumber" };
        try
        {
            csv.Read();
            if(!csv.ReadHeader())
            {
                return false;
            }
            
            var csvHeader = csv.HeaderRecord ?? new List<string>().ToArray();
            headers = headers.Select(h => h.Trim().ToUpper()).ToList();
            foreach (var h in csvHeader)
            {
                if (!headers.Contains(h.Trim().ToUpper()))
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private IEnumerable<Employee> ProcessEmployeeRecords(List<CsvUploadRecord> recordList)
    {
        // check for nulls
        // Use TryParse instead
        // We need to save the unprocessed ones, or return an additional
        // collection
        var employees = recordList
           .Select(e => new Employee(e.EmployeeNumber,
                                     e.EmployeeFirstName,
                                     e.EmployeeLastName,
                                     e.EmployeeEmail,
                                     e.EmployeeDepartment,
                                     string.IsNullOrWhiteSpace(e.HireDate) ? DateTime.MinValue : DateTime.Parse(e.HireDate),
                                     e.ManagerEmployeeNumber,
                                     int.Parse(e.CompanyId)))
           .ToList();

        // validate employee company affiliation (employee number is unique per company) 
        // move to employee service
        var validEmployeeCompanyAffiliationResults = _employeeService.ValidateEmployees(employees);

        // get non-managed employees (top-level employees)
        var nonManagedEmployees = employees.Where(e => string.IsNullOrWhiteSpace(e.ManagerEmployeeNumber));

        //move to employee service
        var employeeManagerCompanyAffiliationResults = _employeeService.ValidateEmployeeManagerCompanyAffiliation(validEmployeeCompanyAffiliationResults.Valid.Values, employees);
        var validEmployeeManagerCompanyAffiliation = employeeManagerCompanyAffiliationResults.Valid.ToList();
        validEmployeeManagerCompanyAffiliation.AddRange(nonManagedEmployees);
        return validEmployeeManagerCompanyAffiliation;
    }

    private IEnumerable<Company> GetCompanyRecords(IEnumerable<CsvUploadRecord> parsedRecords)
    {
        // Add exception handling
        // Traverse parsed records and project into the company collection
        // Now we have to group by company to eliminate duplicate records
        var companyCollection = parsedRecords
           .GroupBy(x => new { x.CompanyId, x.CompanyCode, x.CompanyDescription })
           .Select(g => new Company(int.Parse(g.Key.CompanyId), g.Key.CompanyCode, g.Key.CompanyDescription))
           .ToList();

        return companyCollection;
    }

    private async Task<OperationResult> SaveFileUpload(IEnumerable<CsvUploadRecord> parsedRecords)
    {
        var timeStamp = DateTime.Now;
        var runId = Guid.NewGuid();
        // copy parsed record into a list of file uploads
        var fileUploads = parsedRecords.ToList()
                        .Select(f => new RepoModels.FileUpload()
                        {
                            RunId = runId,
                            CompanyId = f.CompanyId,
                            CompanyCode = f.CompanyCode,
                            CompanyDescription = f.CompanyDescription,
                            EmployeeNumber = f.EmployeeNumber,
                            EmployeeFirstName = f.EmployeeFirstName,
                            EmployeeLastName = f.EmployeeLastName,
                            EmployeeEmail = f.EmployeeEmail,
                            EmployeeDepartment = f.EmployeeDepartment,
                            HireDate = string.IsNullOrWhiteSpace(f.HireDate) ? null : f.HireDate,
                            ManagerEmployeeNumber = f.ManagerEmployeeNumber,
                            CreatedAt = timeStamp
                        });

        // Save CSV raw file
        return await _fileUploadRepository.SaveAllAsync(fileUploads);
    }

    private async Task<OperationResult> SaveCompanyRecords(IEnumerable<Company> companyRecords)
    {
        return await _companyService.SaveCompaniesAsync(companyRecords);
    }

    private async Task<OperationResult> SaveEmployeeRecords(IEnumerable<Employee> employeeRecords)
    {
        return await _employeeService.SaveEmployeesAsync(employeeRecords);
    }

    public async Task<OperationResult> SaveUpload(RawUploadDto upload)
    {
        var result = new OperationResult(OperationResults.Sucessfull, string.Empty);
        var uploadModel = new Upload(Guid.NewGuid(), upload.RawLine.Split(Environment.NewLine));

        // Parse Raw Upload into CSV file
        var parsedRecords = ParseCsvFile(upload);
        if (parsedRecords == null)
        {
            result.Result = OperationResults.Failure;
            result.Message = "Unable to Process CSV file";
            return result;
        }

        // Save upload to repository
        result = await SaveFileUpload(parsedRecords);
        if (result.Result == OperationResults.Failure)
        {
            result.Message = "Unable to Process CSV file. " + result.Message;
            return result;
        }

        // Create company Records
        var recordList = parsedRecords.ToList();
        var companyRecords = GetCompanyRecords(recordList);

        // var save company records to repository
        result = await SaveCompanyRecords(companyRecords);
        if (result.Result == OperationResults.Failure)
        {
            result.Message = "Unable to Process CSV file. " + result.Message;
            return result;
        }

        // Create employee Records
        // Verify that the employee number is unique per company
        // Verify that the employee's manager is part of the company
        // Include top level managers, employees without a managerEmployeeNumber
        var employeeRecords = ProcessEmployeeRecords(recordList);

        // var save employee records to repository
        result = await SaveEmployeeRecords(employeeRecords);
        if (result.Result == OperationResults.Failure)
        {
            result.Message = "Unable to Process CSV file. " + result.Message;
            return result;
        }

        result.Message = $"Company Records Imported {companyRecords.Count()}. Employee Records Imported {employeeRecords.Count()} ";

        return result;
    }

    public async Task<OperationResult> ClearUploadData()
    {
        var result = await _fileUploadRepository.DeleteAllAsync();
        var employeeResult = await _employeeService.DeleteAllAsync();
        var compayResult = await _companyService.DeleteAllAsync();
        return result;
    }
}