using vvt.common.lib.models;
using vvt.services.lib.models;
using RepoModels = vvt.services.repositories.lib.models;
using vvt.services.repositories.lib;
using vvt.common.lib.dtos;

namespace vvt.services.lib;

public class CompanyService : ICompanyService
{

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    private readonly ICompanyRepository _companyRepository;

    public async Task<OperationResult> SaveCompaniesAsync(IEnumerable<Company> companies)
    {
        var timeStamp = DateTime.Now;
        var comps = companies.ToList()
                        .Select(c => new RepoModels.Company()
                        {
                            CompanyId = c.CompanyId,
                            CompanyCode = c.CompanyCode,
                            CompanyDescription = c.CompanyDescrption,
                            CreatedAt = timeStamp
                        });

        return await _companyRepository.SaveAllAsync(comps);
    }

    public async Task<OperationResult> DeleteAllAsync()
    {
        return await _companyRepository.DeleteAllAsync();
    }

    public async Task<IEnumerable<CompanyHeaderDto>> GetCompaniesAsync()
    {
        /*
        SELECT comp.Company_Id, comp.Company_Code, comp.Company_Description, COUNT(emp.Company_Id) FROM dbo.Companies comp
        INNER JOIN dbo.Employees emp
            ON comp.Company_Id = emp.Company_Id
        GROUP BY comp.Company_Id, Comp.Company_Code, comp.Company_Description
        */
        var companies = await _companyRepository.GetAllAsync();
        var companyHeaders = companies
                                .Select(c => new CompanyHeaderDto 
                                    {
                                        Id = c.CompanyId,
                                        Code = c.CompanyCode,
                                        Description = c.CompanyDescription,
                                        EmployeeCount = c.Employees.Count
                                    })
                                .ToList();
        return companyHeaders;
    }

    public async Task<CompanyDto?> GetCompanyAsync(int companyId)
    {
        var comp = await _companyRepository.GetByIdAsync(companyId);
        if (comp == null)
        {
            return null;
        }

        var employees = comp.Employees
        .Select(c => new EmployeeHeaderDto 
            {
                EmployeeNumber = c.EmployeeNumber,
                FullName = $"{c.EmployeeFirstName} {c.EmployeeLastName}"
            }
        );
        var companyDto = new CompanyDto() 
        {
            Id = comp.CompanyId,
            Code = comp.CompanyCode,
            Description = comp.CompanyDescription,
            EmployeeCount = comp.Employees.Count,
            Employees = employees.ToList()
        };

        return companyDto;
    }
}