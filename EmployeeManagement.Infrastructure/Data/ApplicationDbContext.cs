using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
  
  public DbSet<Employee> Employees { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Employee>(entity => {  
      entity.HasKey(e => e.Id);
      entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
      entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
      entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
      entity.Property(e => e.PhoneNumber).HasMaxLength(20);
      entity.Property(e => e.Department).HasMaxLength(100);
      entity.Property(e => e.Position).HasMaxLength(100);
      entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");  
      entity.Property(e => e.HireDate).IsRequired();
    });
  }
}