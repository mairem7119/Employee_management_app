using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Core.Interfaces;

public interface IEmployeeRepository
{
  Task<IEnumerable<Employee>> GetAllAsync(); 
  Task<Employee?> GetByIdAsync(int id);  
  Task<Employee> AddAsync(Employee employee);
  Task<Employee> UpdateAsync(Employee employee);
  Task<bool> DeleteAsync(int id);
  Task<bool> ExistsAsync(int id);
}