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

  public async Task<IEnumerable<Employee>> GetAllEmployees()
  {
    return await _context.Employees.ToListAsync();
  }

  public async Task<Employee> GetByIDAsync(int id)
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
    var employee = _context.Employees.GetByIDAsync(employee.Id);

    if(employee == null) throw new NotFoundException("Employee not found");

    employee.UpdatedAt = DateTime.UtcNow;
    _context.Employees.Update(employee);
    await _context.SaveChangesAsync();
    return employee;
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
    return await _context.Employees.AddAsync(e => e.Id == id);
  }
}