using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
  
  public DbSet<Employee> Employees {get; set;}

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Employee>(employee => {
      entity.HasKey(e => e.Id);
      entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
      entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
      entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
      entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(15);
      entity.Property(e => e.Department).IsRequired().HasMaxLength(50);
      entity.Property(e => e.Position).IsRequired().HasMaxLength(50);
      entity.Property(e => e.Salary).IsRequired().HasMaxLength(10);
    });
  }
}