using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
  private readonly IDepartmentService _departmentService;

  public DepartmentController(IDepartmentService departmentService)
  {
    _departmentService = departmentService;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Department>>> GetAllDepartments()
  {
    var departments = await _departmentService.GetAllDepartmentsAsync();
    return Ok(departments);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Department>> GetDepartmentById(int id)
  {
    var department = await _departmentService.GetDepartmentByIdAsync(id);
    if(department == null) return NotFound();
    return Ok(department);
  }

  [HttpPost]
  public async Task<ActionResult<Department>> CreateDepartment([FromBody] Department department)
  {
    if(!ModelState.IsValid) return BadRequest(ModelState);
    var createdDepartment = await _departmentService.CreateDepartmentAsync(department);
    return CreatedAtAction(nameof(GetDepartmentById), new{id = createdDepartment.Id}, createdDepartment);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Department>> UpdateDepartment(int id, [FromBody] Department department)
  {
    if(!ModelState.IsValid) return BadRequest(ModelState);
    var updatedDepartment = await _departmentService.UpdateDepartmentAsync(id, department);
    if(updatedDepartment == null) return NotFound();
    return Ok(updatedDepartment);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteDepartment(int id)
  {
    var deleted = await _departmentService.DeleteDepartmentAsync(id);
    if(!deleted) return NotFound();
    return NoContent();
  }
}