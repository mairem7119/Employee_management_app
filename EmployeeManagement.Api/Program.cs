using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Interfaces;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using EmployeeManagement.Core.Entities;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Database - PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>(); 
// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
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

// Apply migrations và seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        Console.WriteLine("🔍 Checking database connection...");
        
        // Kiểm tra kết nối
        if (!context.Database.CanConnect())
        {
            throw new Exception("Cannot connect to PostgreSQL database. Please check your connection string and ensure PostgreSQL is running.");
        }
        
        Console.WriteLine("✅ Database connection successful!");
        
        // Kiểm tra và apply migrations
        try
        {
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"📦 Applying {pendingMigrations.Count()} pending migration(s):");
                foreach (var migration in pendingMigrations)
                {
                    Console.WriteLine($"   - {migration}");
                }
                context.Database.Migrate();
                Console.WriteLine("✅ Migrations applied successfully!");
            }
            else
            {
                Console.WriteLine("✅ Database is up to date.");
            }
        }
        catch (Exception migrationEx)
        {
            Console.WriteLine($"⚠️  Migration warning: {migrationEx.Message}");
            // Kiểm tra xem bảng đã tồn tại chưa
            try
            {
                var tableExists = context.Database.ExecuteSqlRaw("SELECT 1 FROM \"Employees\" LIMIT 1") >= 0;
                if (tableExists)
                {
                    Console.WriteLine("✅ Tables already exist. Continuing...");
                }
            }
            catch
            {
                Console.WriteLine("❌ Tables do not exist. Please run: dotnet ef database update");
                throw;
            }
        }
        
        // Seed data mẫu
        try
        {
            if (!context.Employees.Any())
            {
                Console.WriteLine("🌱 Seeding initial data...");
                Console.WriteLine("ℹ️  Seed data skipped. Please add employees via Web UI.");
                // context.Employees.AddRange(
                //     new Employee
                //     {
                //         FirstName = "Nguyễn",
                //         LastName = "Văn A",
                //         Email = "nguyenvana@example.com",
                //         PhoneNumber = "0123456789",
                //         Department = "IT",
                //         Position = "Developer",
                //         Salary = 15000000,
                //         HireDate = DateTime.Now.AddYears(-2),
                //         CreatedAt = DateTime.UtcNow
                //     },
                //     new Employee
                //     {
                //         FirstName = "Trần",
                //         LastName = "Thị B",
                //         Email = "tranthib@example.com",
                //         PhoneNumber = "0987654321",
                //         Department = "HR",
                //         Position = "Manager",
                //         Salary = 20000000,
                //         HireDate = DateTime.Now.AddYears(-1),
                //         CreatedAt = DateTime.UtcNow
                //     },
                //     new Employee
                //     {
                //         FirstName = "Lê",
                //         LastName = "Văn C",
                //         Email = "levanc@example.com",
                //         PhoneNumber = "0912345678",
                //         Department = "Finance",
                //         Position = "Accountant",
                //         Salary = 12000000,
                //         HireDate = DateTime.Now.AddMonths(-6),
                //         CreatedAt = DateTime.UtcNow
                //     }
                // );
                // context.SaveChanges();
                // Console.WriteLine("✅ Seed data đã được thêm vào database!");
            }
            else
            {
                Console.WriteLine("ℹ️  Database already contains data. Skipping seed.");
            }
        }
        catch (Exception seedEx)
        {
            Console.WriteLine($"⚠️  Seed data warning: {seedEx.Message}");
        }
    }
    catch (NpgsqlException ex)
    {
        Console.WriteLine($"\n❌ PostgreSQL Error: {ex.Message}");
        Console.WriteLine($"Error Code: {ex.SqlState}");
        Console.WriteLine("\n💡 Possible solutions:");
        Console.WriteLine("1. Check if PostgreSQL service is running");
        Console.WriteLine("2. Verify connection string in appsettings.json");
        Console.WriteLine("3. Ensure database 'EmployeeManagementDb' exists");
        Console.WriteLine("4. Check username and password");
        Console.WriteLine("5. Run: dotnet ef database update");
        throw;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ Error: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        }
        throw;
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

Console.WriteLine("\n🚀 API is starting...");
app.Run();