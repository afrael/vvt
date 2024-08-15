using vvt.common.lib.dtos;
using vvt.common.lib.models;
using vvt.services.lib.models;

namespace vvt.services.lib;

public interface IEmployeeService
{
    Task<EmployeeDto?> GetEmployeeByCompanyIdAndEmployeeNumberAsync(int companyId, string employeeNumber);
    Task<OperationResult> SaveEmployeesAsync(IEnumerable<Employee> employees);
    Task<OperationResult> DeleteAllAsync();
}