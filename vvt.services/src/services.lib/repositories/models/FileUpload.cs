using System;
using System.Collections.Generic;

namespace vvt.services.repositories.lib.models;

public partial class FileUpload
{
    public int Id { get; set; }

    public Guid RunId { get; set; }

    public string? CompanyId { get; set; }

    public string? CompanyCode { get; set; }

    public string? CompanyDescription { get; set; }

    public string? EmployeeNumber { get; set; }

    public string? EmployeeFirstName { get; set; }

    public string? EmployeeLastName { get; set; }

    public string? EmployeeEmail { get; set; }

    public string? EmployeeDepartment { get; set; }

    public string? HireDate { get; set; }

    public string? ManagerEmployeeNumber { get; set; }

    public DateTime CreatedAt { get; set; }
}
