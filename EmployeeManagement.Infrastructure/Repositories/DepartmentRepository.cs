using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Interfaces;
using EmployeeManagement.Infrastructure.Data;

namespace EmployeeManagement.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
  private readonly ApplicationDbContext _context;

  public DepartmentRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Department>> GetAllAsync()
  {
    return await _context.Departments.AsNoTracking().ToListAsync();
  }

  public async Task<IEnumerable<Department>> GetAllActiveAsync()
  {
    return await _context.Departments
        .AsNoTracking()
        .Where(d => d.IsActive == "true")
        .ToListAsync();
  }

  public async Task<Department?> GetByIdAsync(int id)
  {
    return await _context.Departments.FindAsync(id);
  }

  public async Task<Department> AddAsync(Department department)
  {
    department.CreatedAt = DateTime.UtcNow;
    department.UpdatedAt = DateTime.UtcNow;
    
    _context.Departments.Add(department);
    await _context.SaveChangesAsync();
    return department;
  }

  public async Task<Department> UpdateAsync(Department department)
  {
    var existing = await _context.Departments.FindAsync(department.Id);
    if(existing == null) 
      throw new InvalidOperationException("Department not found");

    existing.Code = department.Code;
    existing.Name = department.Name;
    existing.Description = department.Description;
    existing.IsActive = department.IsActive;
    existing.UpdatedAt = DateTime.UtcNow;
    
    _context.Departments.Update(existing);
    await _context.SaveChangesAsync();
    return existing;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var department = await _context.Departments.FindAsync(id);
    if(department == null) return false;

    _context.Departments.Remove(department);
    await _context.SaveChangesAsync();
    return true;
  }

  public async Task<bool> ExistsAsync(int id)
  {
    return await _context.Departments.AnyAsync(d => d.Id == id);
  }

  public async Task<IEnumerable<Department>> SearchAsync(string? searchTerm)
  {
    var query = _context.Departments.AsQueryable();

    if(!string.IsNullOrWhiteSpace(searchTerm))
    {
      searchTerm = searchTerm.Trim();
      query = query.Where(d => d.Name.Contains(searchTerm) || d.Code.Contains(searchTerm));
    }

    return await query.AsNoTracking().OrderBy(d => d.Name).ToListAsync();
  }
}