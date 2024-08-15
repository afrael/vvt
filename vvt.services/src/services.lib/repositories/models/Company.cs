using System;
using System.Collections.Generic;

namespace vvt.services.repositories.lib.models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string? CompanyCode { get; set; }

    public string? CompanyDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
}
