using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Interfaces;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using EmployeeManagement.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Database - Dùng In-Memory cho development (không cần SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("EmployeeManagementDb"));

// Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed data mẫu cho In-Memory database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Kiểm tra xem đã có data chưa
    if (!context.Employees.Any())
    {
        context.Employees.AddRange(
            new Employee
            {
                FirstName = "Nguyễn",
                LastName = "Văn A",
                Email = "nguyenvana@example.com",
                PhoneNumber = "0123456789",
                Department = "IT",
                Position = "Developer",
                Salary = 15000000,
                HireDate = DateTime.Now.AddYears(-2),
                CreatedAt = DateTime.UtcNow
            },
            new Employee
            {
                FirstName = "Trần",
                LastName = "Thị B",
                Email = "tranthib@example.com",
                PhoneNumber = "0987654321",
                Department = "HR",
                Position = "Manager",
                Salary = 20000000,
                HireDate = DateTime.Now.AddYears(-1),
                CreatedAt = DateTime.UtcNow
            },
            new Employee
            {
                FirstName = "Lê",
                LastName = "Văn C",
                Email = "levanc@example.com",
                PhoneNumber = "0912345678",
                Department = "Finance",
                Position = "Accountant",
                Salary = 12000000,
                HireDate = DateTime.Now.AddMonths(-6),
                CreatedAt = DateTime.UtcNow
            }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.UseRouting();  
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();