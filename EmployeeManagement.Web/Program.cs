using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Interfaces;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database - PostgreSQL (dùng chung với API)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

var app = builder.Build();

// Test database connection
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        // Kiểm tra kết nối database
        if (!context.Database.CanConnect())
        {
            throw new Exception("Cannot connect to PostgreSQL database. Please check your connection string.");
        }
        Console.WriteLine("✅ Database connection successful!");
    }
    catch (NpgsqlException ex)
    {
        Console.WriteLine($"❌ PostgreSQL Error: {ex.Message}");
        Console.WriteLine($"Error Code: {ex.SqlState}");
        Console.WriteLine("\n💡 Please check:");
        Console.WriteLine("1. PostgreSQL service is running");
        Console.WriteLine("2. Connection string in appsettings.json is correct");
        Console.WriteLine("3. Database 'EmployeeManagementDb' exists");
        // Không throw để app vẫn chạy, nhưng sẽ lỗi khi truy cập Employees
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Trong Development, hiển thị chi tiết lỗi
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();