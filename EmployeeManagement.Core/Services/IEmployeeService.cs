using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Core.Services;

public interface IEmployeeService
{
  Task<IEnumerable<Employee>> GetAllEmployeesAsync();
  Task<Employee?> GetEmployeeByIdAsync(int id);  // Thêm ?
  Task<Employee> CreateEmployeeAsync(Employee employee);
  Task<Employee?> UpdateEmployeeAsync(int id, Employee employee);  // Thêm ?
  Task<bool> DeleteEmployeeAsync(int id);
}