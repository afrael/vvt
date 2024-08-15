using vvt.common.lib.models;
using vvt.common.lib.enums;
using Microsoft.EntityFrameworkCore;
using System.Data;
using vvt.services.repositories.lib.models;

namespace vvt.services.repositories.lib;

public class EmployeeRepository : IEmployeeRepository
{
    public async Task<OperationResult> SaveAllAsync(IEnumerable<Employee> employees)
    {
        var operationResult = new OperationResult(OperationResults.Sucessfull);

        using (var db = new VvtContext())
        {
            // add then new records
            foreach (var e in employees)
            {
                db.Employees.Add(e);
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
            var allEmployees = await db.Employees.ToListAsync();
            db.RemoveRange(allEmployees);
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

    public async Task<Employee?> GetByEmployeeNumberAndCompanyIdAsync(string employeeNumber, int companyId)
    {
        using (var db = new VvtContext())
        {
            var result = await db.Employees
                .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber && e.CompanyId == companyId);
            
            return result;
        }
    }

    public async Task<Employee?> GetByEmployeeNumber(string employeeNumber)
    {
        using (var db = new VvtContext())
        {
            var result = await db.Employees
                .FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
            
            return result;
        }
    }
}