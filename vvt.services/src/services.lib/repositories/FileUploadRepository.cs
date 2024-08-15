using vvt.common.lib.models;
using vvt.common.lib.enums;
using Microsoft.EntityFrameworkCore;
using System.Data;
using vvt.services.repositories.lib.models;

namespace vvt.services.repositories.lib;

public class FileUploadRepository : IFileUploadRepository
{
    public async Task<OperationResult> SaveAllAsync(IEnumerable<FileUpload> fileUploads)
    {
        var operationResult = new OperationResult(OperationResults.Sucessfull);

        using (var db = new VvtContext())
        {
            // add then new records
            foreach (var f in fileUploads)
            {
                db.FileUploads.Add(f);
            }
            try
            {
                var result = await db.SaveChangesAsync();
                if (result > 0)
                {
                    operationResult.Message = $"Total Records Added or Updated : {result}";
                }
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateException || 
                    ex is DbUpdateConcurrencyException ||
                    ex is DBConcurrencyException)
                {
                    return new OperationResult(OperationResults.Failure, $"Error while trying to save the results. Exception: {ex.Message}");
                }
            }
            return operationResult;
        }
    }
    
    public async Task<OperationResult> DeleteAllAsync()
    {
        var operationResult = new OperationResult(OperationResults.Sucessfull);
        using (var db = new VvtContext())
        {
            var allFileUploads = await db.FileUploads.ToListAsync();
            db.RemoveRange(allFileUploads);
            try
            {
                var result = await db.SaveChangesAsync();
                if (result > 0)
                {
                    operationResult.Message = $"Total Records Removed : {result}";
                }
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateException || 
                    ex is DbUpdateConcurrencyException ||
                    ex is DBConcurrencyException)
                {
                    return new OperationResult(OperationResults.Failure, $"Error while trying to save the results. Exception: {ex.Message}");
                }
            }
            return operationResult;
        }
    }
}