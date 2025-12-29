using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
  
  public DbSet<Employee> Employees { get; set; }

  public DbSet<Department> Departments {get; set;}

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Employee>(entity => {  
      entity.HasKey(e => e.Id);
      
      entity.Property(e => e.FirstName)
          .IsRequired()
          .HasMaxLength(100);
      
      entity.Property(e => e.LastName)
          .IsRequired()
          .HasMaxLength(100);
      
      entity.Property(e => e.Email)
          .IsRequired()
          .HasMaxLength(200);
      
      entity.Property(e => e.PhoneNumber)
          .IsRequired()
          .HasMaxLength(20);
      
      entity.Property(e => e.DepartmentId)
          .IsRequired();
      
      entity.Property(e => e.Position)
          .IsRequired()
          .HasMaxLength(100);
      
      entity.Property(e => e.Salary)
          .IsRequired()
          .HasColumnType("numeric(18,2)");
      
      entity.Property(e => e.HireDate)
          .IsRequired();
      
      entity.HasIndex(e => e.Email)
          .IsUnique()
          .HasDatabaseName("IX_Employees_Email");

      entity.HasOne(e => e.Department)
        .WithMany()
        .HasForeignKey(e => e.DepartmentId)
        .OnDelete(DeleteBehavior.Restrict);// Không cho xóa Department nếu có Employee
    });

    modelBuilder.Entity<Department>(entity => {
        entity.HasKey(d => d.Id);

        entity.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(10);

        entity.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(d => d.Description)
            .HasMaxLength(500);

        entity.Property(d => d.IsActive)
            .IsRequired()
            .HasMaxLength(10);

        entity.Property(d => d.CreatedAt)
            .IsRequired();

        entity.Property(d => d.UpdatedAt)
            .IsRequired();

        entity.HasIndex(d => d.Code)
            .IsUnique()
            .HasDatabaseName("IX_Departments_Code");

        entity.HasIndex(d => d.Name)
            .IsUnique()
            .HasDatabaseName("IX_Departments_Name");
    });
  }
}