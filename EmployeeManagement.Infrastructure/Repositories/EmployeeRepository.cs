using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Interfaces;
using EmployeeManagement.Infrastructure.Data;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
  private readonly ApplicationDbContext _context;

  public EmployeeRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Employee>> GetAllAsync()
  {
    return await _context.Employees
    .Include(e => e.Department)
    .AsNoTracking()
    .ToListAsync();
  }

  public async Task<Employee?> GetByIdAsync(int id)
  {
    return await _context.Employees
    .Include(e => e.Department)
    .FirstOrDefaultAsync(e => e.Id == id);
  }

  public async Task<Employee> AddAsync(Employee employee)
  {
    try
    {
      // Convert HireDate sang UTC nếu chưa phải UTC
      if (employee.HireDate.Kind != DateTimeKind.Utc)
      {
        employee.HireDate = DateTime.SpecifyKind(employee.HireDate.Date, DateTimeKind.Utc);
      }
      
      // Đảm bảo CreatedAt được set và là UTC
      employee.CreatedAt = DateTime.UtcNow;
      
      _context.Employees.Add(employee);
      await _context.SaveChangesAsync();
      return employee;
    }
    catch (DbUpdateException ex)
    {
      var innerException = ex.InnerException;
      if (innerException != null)
      {
        throw new InvalidOperationException(
          $"Database error: {innerException.Message}. " +
          $"Original error: {ex.Message}", ex);
      }
      throw;
    }
  }

  public async Task<Employee> UpdateAsync(Employee employee)
  {
    var existingEmployee = await _context.Employees.FindAsync(employee.Id);

    if(existingEmployee == null) 
      throw new InvalidOperationException("Employee not found");

    existingEmployee.FirstName = employee.FirstName;
    existingEmployee.LastName = employee.LastName;
    existingEmployee.Email = employee.Email;
    existingEmployee.PhoneNumber = employee.PhoneNumber;
    existingEmployee.DepartmentId = employee.DepartmentId;
    existingEmployee.Position = employee.Position;
    existingEmployee.Salary = employee.Salary;
    
    // Convert HireDate sang UTC
    if (employee.HireDate.Kind != DateTimeKind.Utc)
    {
      existingEmployee.HireDate = DateTime.SpecifyKind(employee.HireDate.Date, DateTimeKind.Utc);
    }
    else
    {
      existingEmployee.HireDate = employee.HireDate;
    }
    
    existingEmployee.UpdatedAt = DateTime.UtcNow;
    
    try
    {
      _context.Employees.Update(existingEmployee);
      await _context.SaveChangesAsync();
      return existingEmployee;
    }
    catch (DbUpdateException ex)
    {
      var innerException = ex.InnerException;
      if (innerException != null)
      {
        throw new InvalidOperationException(
          $"Database error: {innerException.Message}. " +
          $"Original error: {ex.Message}", ex);
      }
      throw;
    }
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var employee = await _context.Employees.FindAsync(id);

    if(employee == null) return false;

    _context.Employees.Remove(employee);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<bool> ExistsAsync(int id)
  {
    return await _context.Employees.AnyAsync(e => e.Id == id);
  }

  public async Task<IEnumerable<Employee>> SearchAsync(string? searchTerm, int? departmentId, string? position)
  {
    var query = _context.Employees.Include(e => e.Department).AsQueryable();

    // Tìm kiếm theo từ khóa (tên, email, số điện thoại)
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
      searchTerm = searchTerm.Trim();
      query = query.Where(e => 
        e.FirstName.Contains(searchTerm) ||
        e.LastName.Contains(searchTerm) ||
        e.Email.Contains(searchTerm) ||
        e.PhoneNumber.Contains(searchTerm));
    }

    // Lọc theo phòng ban
    if (departmentId.HasValue && departmentId.Value > 0)
    {
      query = query.Where(e => e.DepartmentId == departmentId.Value);
    }

    // Lọc theo chức vụ
    if (!string.IsNullOrWhiteSpace(position))
    {
      query = query.Where(e => e.Position.Contains(position));
    }

    return await query
      .AsNoTracking()
      .OrderBy(e => e.FirstName)
      .ThenBy(e => e.LastName)
      .ToListAsync();
  }
}