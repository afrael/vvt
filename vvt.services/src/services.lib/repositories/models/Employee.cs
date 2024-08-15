using System;
using System.Collections.Generic;

namespace vvt.services.repositories.lib.models;

public partial class Employee
{
    public int Id { get; set; }

    public string? EmployeeNumber { get; set; }

    public string? EmployeeFirstName { get; set; }

    public string? EmployeeLastName { get; set; }

    public string? EmployeeEmail { get; set; }

    public string? EmployeeDepartment { get; set; }

    public DateTime HireDate { get; set; }

    public string? ManagerEmployeeNumber { get; set; }

    public int CompanyId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;
}
