using vvt.common.lib.models;
using vvt.services.lib.models;
using RepoModels = vvt.services.repositories.lib.models;
using vvt.services.repositories.lib;
using vvt.common.lib.dtos;

namespace vvt.services.lib;

public class EmployeeService : IEmployeeService
{
    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    private readonly IEmployeeRepository _employeeRepository;

    public async Task<OperationResult> SaveEmployeesAsync(IEnumerable<Employee> employees)
    {
        var timeStamp = DateTime.Now;
        var emps = employees.ToList()
                        .Select(e => new RepoModels.Employee()
                        {
                            CompanyId = e.CompanyId,
                            EmployeeNumber = e.EmployeeNumber,
                            EmployeeFirstName = e.FirstName,
                            EmployeeLastName = e.LastName,
                            EmployeeEmail = e.Email,
                            EmployeeDepartment = e.Department,
                            ManagerEmployeeNumber = e.ManagerEmployeeNumber,
                            HireDate = e.HireDate,
                            CreatedAt = timeStamp
                        });

        return await _employeeRepository.SaveAllAsync(emps);
    }

    public async Task<OperationResult> DeleteAllAsync()
    {
        return await _employeeRepository.DeleteAllAsync();
    }

    public async Task<EmployeeDto?> GetEmployeeByCompanyIdAndEmployeeNumberAsync(int companyId, string employeeNumber)
    {
        var employee = await _employeeRepository.GetByEmployeeNumberAndCompanyIdAsync(employeeNumber, companyId);
        if (employee == null)
        {
            return null;
        }

        var managers = await GetHierarchicalListOfManagers(employee);
        var empDto = new EmployeeDto()
        {
            EmployeeNumber = employee.EmployeeNumber,
            FullName = $"{employee.EmployeeFirstName} {employee.EmployeeLastName}",
            Email = employee.EmployeeEmail,
            Department = employee.EmployeeDepartment,
            HireDate = employee.HireDate,
            Managers = managers.ToList()
        };
        return empDto;
    }

    private async Task<IEnumerable<EmployeeHeaderDto>> GetHierarchicalListOfManagers(RepoModels.Employee employee)
    {
        var managers = new List<EmployeeHeaderDto>();
        while(!string.IsNullOrWhiteSpace(employee.ManagerEmployeeNumber))
        {
            var mgr = await _employeeRepository.GetByEmployeeNumber(employee.ManagerEmployeeNumber);
            if (mgr == null)
            {
                break;
            }
            
            var mgrDto = new EmployeeHeaderDto()
            {
                EmployeeNumber = mgr.EmployeeNumber ?? "",
                FullName = $"{mgr.EmployeeFirstName} {mgr.EmployeeLastName}",
            };
            managers.Add(mgrDto);
            employee.ManagerEmployeeNumber = mgr.ManagerEmployeeNumber;
        }

        return managers;
    }
}