using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Core.Services;

public interface IEmployeeService
{
  Task<IEnumerable<Employee>> GetAllEmployeesAsync();
  Task<Employee?> GetEmployeeByIdAsync(int id); 
  Task<Employee> CreateEmployeeAsync(Employee employee);
  Task<Employee?> UpdateEmployeeAsync(int id, Employee employee);  
  Task<bool> DeleteEmployeeAsync(int id);
  Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm, int? departmentId, string? position);
}