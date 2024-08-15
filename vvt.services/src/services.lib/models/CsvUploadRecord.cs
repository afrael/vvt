namespace vvt.services.lib.models;

public class CsvUploadRecord
{
    public string CompanyId { get; set; } = "";

    public string CompanyCode {get; set; } = "";

    public string CompanyDescription {get; set; } = "";

    public string EmployeeNumber {get; set; } = "";

    public string EmployeeFirstName {get; set; } = "";

    public string EmployeeLastName {get; set; } = "";

    public string EmployeeEmail {get; set; } = "";

    public string EmployeeDepartment {get; set; } = "";

    public string HireDate {get; set; } = "";

    public string ManagerEmployeeNumber {get; set; } = "";
}