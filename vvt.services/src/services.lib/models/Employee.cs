namespace vvt.services.lib.models;

public class Employee
{
    public string EmployeeNumber { get; set; }
    
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email {get; set; }

    public string Department {get; set; }

    public DateTime HireDate {get; set;}

    public string ManagerEmployeeNumber {get; set; }

    public int CompanyId { get; set; }

    public Employee(string employeeNumber, 
        string firstName, 
        string lastName,
        string email,
        string department,
        DateTime hireDate,
        string managerEmployeeNumber,
        int companyId)
        {
            EmployeeNumber = employeeNumber;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Department = department;
            HireDate = hireDate;
            ManagerEmployeeNumber = managerEmployeeNumber;
            CompanyId = companyId;
        }
}
