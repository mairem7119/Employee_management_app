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
      
      entity.Property(e => e.FirstName)
          .IsRequired()
          .HasMaxLength(100);
      
      entity.Property(e => e.LastName)
          .IsRequired()
          .HasMaxLength(100);
      
      entity.Property(e => e.Email)
          .IsRequired()
          .HasMaxLength(200);
      
      // THÊM IsRequired() cho PhoneNumber
      entity.Property(e => e.PhoneNumber)
          .IsRequired()
          .HasMaxLength(20);
      
      // THÊM IsRequired() cho Department
      entity.Property(e => e.Department)
          .IsRequired()
          .HasMaxLength(100);
      
      // THÊM IsRequired() cho Position
      entity.Property(e => e.Position)
          .IsRequired()
          .HasMaxLength(100);
      
      entity.Property(e => e.Salary)
          .IsRequired()
          .HasColumnType("numeric(18,2)");
      
      entity.Property(e => e.HireDate)
          .IsRequired();
      
      // Tạo unique index cho Email
      entity.HasIndex(e => e.Email)
          .IsUnique()
          .HasDatabaseName("IX_Employees_Email");
    });
  }
}