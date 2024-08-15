using vvt.services.repositories.lib.models;
using vvt.common.lib.models;

namespace vvt.services.repositories.lib;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllAsync();
    Task<Company?> GetByIdAsync(int companyId);
    Task<OperationResult> SaveAllAsync(IEnumerable<Company> companies);
    Task<OperationResult> DeleteAllAsync();
}
