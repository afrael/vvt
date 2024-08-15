using vvt.common.lib.models;
using vvt.common.lib.enums;
using Microsoft.EntityFrameworkCore;
using System.Data;
using vvt.services.repositories.lib.models;

namespace vvt.services.repositories.lib;

public class CompanyRepository : ICompanyRepository
{
    public async Task<OperationResult> SaveAllAsync(IEnumerable<Company> companies)
    {
        var operationResult = new OperationResult(OperationResults.Sucessfull);

        using (var db = new VvtContext())
        {
            // add then new records
            foreach (var c in companies)
            {
                db.Companies.Add(c);
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
        }
        return operationResult;
    }
    
    public async Task<OperationResult> DeleteAllAsync()
    {
        var operationResult = new OperationResult(OperationResults.Sucessfull);
        using (var db = new VvtContext())
        {
            var allCompanies = await db.Companies.ToListAsync();
            db.RemoveRange(allCompanies);
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

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        using (var db = new VvtContext())
        {
            var result = await db.Companies
                .Include(c => c.Employees)
                .ToListAsync();
            
            return result;
        }
    }

    public async Task<Company?> GetByIdAsync(int companyId)
    {
        using (var db = new VvtContext())
        {
            var result = await db.Companies
                .Include(c => c.Employees)
                .FirstOrDefaultAsync(c => c.CompanyId == companyId);
            
            return result;
        }
    }
}