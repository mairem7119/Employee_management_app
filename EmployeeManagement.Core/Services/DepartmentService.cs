using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Interfaces;

namespace EmployeeManagement.Core.Services;

public class DepartmentService : IDepartmentService
{
  private readonly IDepartmentRepository _departmentRepository;

  public DepartmentService(IDepartmentRepository departmentRepository)
  {
    _departmentRepository = departmentRepository;
  }

  public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
  {
    return await _departmentRepository.GetAllAsync();
  }

  public async Task<IEnumerable<Department>> GetAllActiveDepartmentsAsync()
  {
    return await _departmentRepository.GetAllActiveAsync();
  }

  public async Task<Department?> GetDepartmentByIdAsync(int id)
  {
    return await _departmentRepository.GetByIdAsync(id);
  }

  public async Task<Department> CreateDepartmentAsync(Department department)
  {
    return await _departmentRepository.AddAsync(department);
  }

  public async Task<Department?> UpdateDepartmentAsync(int id, Department department)
  {
    var existingDepartment = await _departmentRepository.GetByIdAsync(id);
    if(existingDepartment == null)
      return null;
    
    existingDepartment.Code = department.Code;
    existingDepartment.Name = department.Name;
    existingDepartment.Description = department.Description;
    existingDepartment.IsActive = department.IsActive;
    
    return await _departmentRepository.UpdateAsync(existingDepartment);
  }

  public async Task<bool> DeleteDepartmentAsync(int id)
  {
    var existingDepartment = await _departmentRepository.GetByIdAsync(id);
    if(existingDepartment == null)
      return false;
    return await _departmentRepository.DeleteAsync(id);
  }
}