namespace vvt.common.lib.dtos;

public class CompanyHeaderDto
{
    public int Id { get; set; }

    public string Code { get; set; } = "";

    public string Description { get; set; } = "";

    public int EmployeeCount { get; set; }
}