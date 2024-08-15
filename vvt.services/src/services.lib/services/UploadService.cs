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

    private IEnumerable<CsvUploadRecord> ParseCsvFile(RawUploadDto rawUpload)
    {
        // Add exception handling
        IEnumerable<CsvUploadRecord> records;
        using (var reader = new StringReader(rawUpload.RawLine))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<CsvUploadRecord>().ToList();
        }

        return records;
    }

    private IEnumerable<Employee> GetEmployeeRecords(List<CsvUploadRecord> recordList)
    {
        // check for nulls
        // Use TryParse instead
        // We need to save the unprocessed ones, or return an additional
        // collection
        var employeeCollection = recordList
           .Select(e => new Employee(e.EmployeeNumber,
                                     e.EmployeeFirstName,
                                     e.EmployeeLastName,
                                     e.EmployeeEmail,
                                     e.EmployeeDepartment,
                                     string.IsNullOrWhiteSpace(e.HireDate) ? DateTime.MinValue : DateTime.Parse(e.HireDate),
                                     e.ManagerEmployeeNumber,
                                     int.Parse(e.CompanyId)))
           .ToList();

        // Use brute force to check that an employeeNumber is unique 
        // per company
        var uniqueEmployeePerCompany = new Dictionary<string, Employee>();
        var rejectedEmployeeDupeCompany = new List<Employee>();
        foreach (var employee in employeeCollection)
        {
            var compKey = $"{employee.CompanyId}-{employee.EmployeeNumber}";
            if (!uniqueEmployeePerCompany.TryAdd(compKey, employee))
            {
                rejectedEmployeeDupeCompany.Add(employee);
            }
        }

        // Use brute force to check that the manager of a given employee
        // should exist in the same company
        var employeesWithoutManager = new List<Employee>();
        var uniqueEmployeePerCompanyAndManagerExistsInCompany = new List<Employee>();
        var rejectedEmployeeMgrNonCompany = new List<Employee>();
        foreach (var employee in uniqueEmployeePerCompany.Values)
        {
            int managersCompanyId = 0;
            // Get Manager's company
            if (!string.IsNullOrWhiteSpace(employee.ManagerEmployeeNumber))
            {
                var manager = employeeCollection
                            .FirstOrDefault(e => e.ManagerEmployeeNumber == employee.ManagerEmployeeNumber);
                if (manager != null)
                {
                    managersCompanyId = manager.CompanyId;
                }
            }
            else
            {
                employeesWithoutManager.Add(employee);
            }

            if (managersCompanyId == employee.CompanyId)
            {
                uniqueEmployeePerCompanyAndManagerExistsInCompany.Add(employee);
            }
            else
            {
                rejectedEmployeeMgrNonCompany.Add(employee);
            }

        }
        uniqueEmployeePerCompanyAndManagerExistsInCompany.AddRange(employeesWithoutManager);
        return uniqueEmployeePerCompanyAndManagerExistsInCompany;
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
        var employeeRecords = GetEmployeeRecords(recordList);

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