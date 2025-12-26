using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Interfaces;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using EmployeeManagement.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database - Dùng In-Memory cho development (giống API)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("EmployeeManagementDb"));

// Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

var app = builder.Build();

// Seed data mẫu (giống API)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();