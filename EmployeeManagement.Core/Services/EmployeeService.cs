using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Interfaces;

namespace EmployeeManagement.Core.Services;

public class EmployeeService : IEmployeeService
{
  private readonly IEmployeeRepository _employeeRepository;

  public EmployeeService(IEmployeeRepository employeeRepository)
  {
    _employeeRepository = employeeRepository;
  }

  public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
  {
    return await _employeeRepository.GetAllAsync();  
  }

  public async Task<Employee?> GetEmployeeByIdAsync(int id) 
  {
    return await _employeeRepository.GetByIdAsync(id);  
  }

  public async Task<Employee> CreateEmployeeAsync(Employee employee)
  {
    return await _employeeRepository.AddAsync(employee);
  }

  public async Task<Employee?> UpdateEmployeeAsync(int id, Employee employee)  
  {
    var existingEmployee = await _employeeRepository.GetByIdAsync(id);  

    if(existingEmployee == null) 
      return null;  

    existingEmployee.FirstName = employee.FirstName;
    existingEmployee.LastName = employee.LastName;
    existingEmployee.Email = employee.Email;
    existingEmployee.PhoneNumber = employee.PhoneNumber;
    existingEmployee.DepartmentId = employee.DepartmentId;
    existingEmployee.Position = employee.Position;
    existingEmployee.Salary = employee.Salary;
    existingEmployee.HireDate = employee.HireDate;
    
    return await _employeeRepository.UpdateAsync(existingEmployee);
  }
  
  public async Task<bool> DeleteEmployeeAsync(int id)
  {
    var existingEmployee = await _employeeRepository.GetByIdAsync(id);  // Sửa tên method

    if(existingEmployee == null) 
      return false;  // Sửa từ throw exception thành return false

    return await _employeeRepository.DeleteAsync(id);
  }

  public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm, int? departmentId, string? position)
  {
    return await _employeeRepository.SearchAsync(searchTerm, departmentId, position);
  }
}