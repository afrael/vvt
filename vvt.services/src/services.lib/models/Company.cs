namespace vvt.services.lib.models;

public class Company
{
    public int CompanyId { get; set; }
    
    public string CompanyCode { get; set; }

    public string CompanyDescrption { get; set; }

    public IEnumerable<Employee> Employees { get; set; }

    public Company(int id, string code, string descrption)
    {
        CompanyId = id;
        CompanyCode = code; 
        CompanyDescrption = descrption;
        Employees = new List<Employee>();
    }
}
