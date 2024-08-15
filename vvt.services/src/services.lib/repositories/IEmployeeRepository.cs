using vvt.services.repositories.lib.models;
using vvt.common.lib.models;

namespace vvt.services.repositories.lib;

public interface IEmployeeRepository
{
    Task<Employee?> GetByEmployeeNumberAndCompanyIdAsync(string employeeNumber, int companyId);
    Task<Employee?> GetByEmployeeNumber(string employeeNumber);
    Task<OperationResult> SaveAllAsync(IEnumerable<Employee> employees);
    Task<OperationResult> DeleteAllAsync();
}
