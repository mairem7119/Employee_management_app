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

  public async Task<IEnumerable<Employee>> GetAllAsync()  // Sửa tên method
  {
    return await _context.Employees.ToListAsync();
  }

  public async Task<Employee?> GetByIdAsync(int id)  // Sửa tên và thêm ?
  {
    return await _context.Employees.FindAsync(id);
  }

  public async Task<Employee> AddAsync(Employee employee)
  {
    employee.CreatedAt = DateTime.UtcNow;
    _context.Employees.Add(employee);
    await _context.SaveChangesAsync();
    return employee;
  }

  public async Task<Employee> UpdateAsync(Employee employee)
  {
    var existingEmployee = await _context.Employees.FindAsync(employee.Id);  // Sửa logic

    if(existingEmployee == null) 
      throw new InvalidOperationException("Employee not found");  // Sửa exception

    existingEmployee.FirstName = employee.FirstName;
    existingEmployee.LastName = employee.LastName;
    existingEmployee.Email = employee.Email;
    existingEmployee.PhoneNumber = employee.PhoneNumber;
    existingEmployee.Department = employee.Department;
    existingEmployee.Position = employee.Position;
    existingEmployee.Salary = employee.Salary;
    existingEmployee.HireDate = employee.HireDate;
    existingEmployee.UpdatedAt = DateTime.UtcNow;
    
    _context.Employees.Update(existingEmployee);
    await _context.SaveChangesAsync();
    return existingEmployee;  // Trả về existingEmployee
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
    return await _context.Employees.AnyAsync(e => e.Id == id);  // Sửa từ AddAsync thành AnyAsync
  }
}