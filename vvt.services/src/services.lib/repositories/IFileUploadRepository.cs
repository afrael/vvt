using vvt.services.repositories.lib.models;
using vvt.common.lib.models;

namespace vvt.services.repositories.lib;

public interface IFileUploadRepository
{
    Task<OperationResult> SaveAllAsync(IEnumerable<FileUpload> fileUploads);
    Task<OperationResult> DeleteAllAsync();
}
