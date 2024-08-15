using vvt.common.lib.dtos;
using vvt.common.lib.models;

namespace vvt.services.lib;

public interface IUploadService
{
    Task<OperationResult> SaveUpload(RawUploadDto upload);
    Task<OperationResult> ClearUploadData();
}