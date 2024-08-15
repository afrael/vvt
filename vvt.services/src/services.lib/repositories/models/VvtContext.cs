using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace vvt.services.repositories.lib.models;

public partial class VvtContext : DbContext
{
    public VvtContext()
    {
    }

    public VvtContext(DbContextOptions<VvtContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<FileUpload> FileUploads { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQL_SERVER_CONSTR"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Companie__5F5D191272F8FC02");

            entity.Property(e => e.CompanyId)
                .ValueGeneratedNever()
                .HasColumnName("Company_Id");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Company_Code");
            entity.Property(e => e.CompanyDescription)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Company_Description");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3213E83F5E3B89FE");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("Company_Id");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EmployeeDepartment)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Employee_Department");
            entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Employee_Email");
            entity.Property(e => e.EmployeeFirstName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Employee_FirstName");
            entity.Property(e => e.EmployeeLastName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Employee_LastName");
            entity.Property(e => e.EmployeeNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Employee_Number");
            entity.Property(e => e.HireDate).HasColumnName("Hire_Date");
            entity.Property(e => e.ManagerEmployeeNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Manager_Employee_Number");

            entity.HasOne(d => d.Company).WithMany(p => p.Employees)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Company_Id");
        });

        modelBuilder.Entity<FileUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FileUplo__3213E83FD696E5E1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Company_Code");
            entity.Property(e => e.CompanyDescription)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Company_Description");
            entity.Property(e => e.CompanyId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Company_Id");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EmployeeDepartment)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Employee_Department");
            entity.Property(e => e.EmployeeEmail)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Employee_Email");
            entity.Property(e => e.EmployeeFirstName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Employee_FirstName");
            entity.Property(e => e.EmployeeLastName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("Employee_LastName");
            entity.Property(e => e.EmployeeNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Employee_Number");
            entity.Property(e => e.HireDate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Hire_Date");
            entity.Property(e => e.ManagerEmployeeNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Manager_Employee_Number");
            entity.Property(e => e.RunId).HasColumnName("Run_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
