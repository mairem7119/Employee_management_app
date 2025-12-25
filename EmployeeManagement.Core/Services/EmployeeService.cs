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
    return await _employeeRepository.GetAllAsync();  // Sửa tên method
  }

  public async Task<Employee?> GetEmployeeByIdAsync(int id)  // Thêm ?
  {
    return await _employeeRepository.GetByIdAsync(id);  // Sửa tên method
  }

  public async Task<Employee> CreateEmployeeAsync(Employee employee)
  {
    return await _employeeRepository.AddAsync(employee);
  }

  public async Task<Employee?> UpdateEmployeeAsync(int id, Employee employee)  // Thêm ?
  {
    var existingEmployee = await _employeeRepository.GetByIdAsync(id);  // Sửa tên method

    if(existingEmployee == null) 
      return null;  // Sửa từ throw exception thành return null

    existingEmployee.FirstName = employee.FirstName;
    existingEmployee.LastName = employee.LastName;
    existingEmployee.Email = employee.Email;
    existingEmployee.PhoneNumber = employee.PhoneNumber;
    existingEmployee.Department = employee.Department;
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
}