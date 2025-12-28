using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Core.Interfaces;

public interface IDepartmentRepository
{
  Task<IEnumerable<Department>> GetAllAsync();
  Task<IEnumerable<Department>> GetAllActiveAsync();
  Task<Department?> GetByIdAsync(int id);
  Task<Department> AddAsync(Department department);
  Task<Department> UpdateAsync(Department department);
  Task<bool> DeleteAsync(int id);
  Task<bool> ExistsAsync(int id);
}