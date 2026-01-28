using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Core.Services;

public interface IDepartmentService
{
  Task<IEnumerable<Department>> GetAllDepartmentsAsync();
  Task<IEnumerable<Department>> GetAllActiveDepartmentsAsync();
  Task<Department?> GetDepartmentByIdAsync(int id);
  Task<Department> CreateDepartmentAsync(Department department);
  Task<Department?> UpdateDepartmentAsync(int id, Department department);
  Task<bool> DeleteDepartmentAsync(int id);
  Task<bool> ExistsAsync(int id);
  Task<IEnumerable<Department>> SearchAsync(string? searchTerm);
}