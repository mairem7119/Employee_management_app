using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Core.Interfaces;

public interface IEmployeeRepository
{
  Task<IEnumerable<Employee>> GetAllAsync();  // Sửa từ GetAllEmployees
  Task<Employee?> GetByIdAsync(int id);  // Sửa từ GetByIDAsync và thêm ?
  Task<Employee> AddAsync(Employee employee);
  Task<Employee> UpdateAsync(Employee employee);
  Task<bool> DeleteAsync(int id);
  Task<bool> ExistsAsync(int id);
}