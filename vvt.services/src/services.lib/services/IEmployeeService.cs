using vvt.common.lib.dtos;
using vvt.common.lib.models;
using vvt.services.lib.models;

namespace vvt.services.lib;

public interface IEmployeeService
{
    Task<EmployeeDto?> GetEmployeeByCompanyIdAndEmployeeNumberAsync(int companyId, string employeeNumber);
    Task<OperationResult> SaveEmployeesAsync(IEnumerable<Employee> employees);
    Task<OperationResult> DeleteAllAsync(); 
    (IDictionary<string, Employee> Valid, IEnumerable<Employee> Invalid) ValidateEmployees(IEnumerable<Employee> employees);
    (IEnumerable<Employee> Valid, IEnumerable<Employee> Invalid) ValidateEmployeeManagerCompanyAffiliation(IEnumerable<Employee> validatedEmployees, IEnumerable<Employee> allEmployees);
}