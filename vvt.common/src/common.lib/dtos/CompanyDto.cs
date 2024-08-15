namespace vvt.common.lib.dtos;

public class CompanyDto : CompanyHeaderDto
{
    public List<EmployeeHeaderDto> Employees { get; set; } = new List<EmployeeHeaderDto>();
}
