using Microsoft.AspNetCore.Mvc;
using vvt.common.lib.dtos;
using vvt.services.lib;

namespace vvt.app.api.Controllers;

[ApiController]
[Route("[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ILogger<DatastoreController> _logger;
    private readonly ICompanyService _companyService;
    private readonly IEmployeeService _employeeService;

    public CompanyController(ILogger<DatastoreController> logger,
        ICompanyService companyService,
        IEmployeeService employeeService)
    {
        _logger = logger;
        _companyService = companyService;
        _employeeService = employeeService;
    }

    [HttpGet("/companies")]
    public async Task<ActionResult<IEnumerable<CompanyHeaderDto>>> GetCompanyHeadersAsync()
    {
        var companyHeaders = await _companyService.GetCompaniesAsync();
        return Ok(companyHeaders);
    }

    [HttpGet("/companies/{companyId}")]
    public async Task<ActionResult<CompanyDto>> GetCompanyByIdAsync(int companyId)
    {
        var company = await _companyService.GetCompanyAsync(companyId);
        if (company == null)
        {
            return NotFound();
        }
        return Ok(company);
    }

    [HttpGet("/companies/{companyId}/employees/{employeeNumber}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeByCompanyIdAndEmployeeNumberAsync(int companyId, string employeeNumber)
    {
        var employee = await _employeeService.GetEmployeeByCompanyIdAndEmployeeNumberAsync(companyId, employeeNumber);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(employee);
    }
}
