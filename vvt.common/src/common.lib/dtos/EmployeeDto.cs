namespace vvt.common.lib.dtos;

public class EmployeeDto : EmployeeHeaderDto
{
    public string Email { get; set; } = "";

    public string Department { get; set; } = "";

    public DateTime HireDate { get; set; }

    public List<EmployeeHeaderDto> Managers { get; set; } = new List<EmployeeHeaderDto>();
}
