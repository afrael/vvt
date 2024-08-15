using vvt.common.lib.models;
using vvt.common.lib.dtos;
using vvt.services.lib.models;

namespace vvt.services.lib;

public interface ICompanyService
{
    Task<OperationResult> SaveCompaniesAsync(IEnumerable<Company> companies);
    Task<OperationResult> DeleteAllAsync();
    Task<IEnumerable<CompanyHeaderDto>> GetCompaniesAsync();
    Task<CompanyDto?> GetCompanyAsync(int companyId);
}