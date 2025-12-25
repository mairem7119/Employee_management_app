using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
  private readonly IEmployeeService _employeeService;

  public EmployeeController(IEmployeeService employeeService)
  {
    _employeeService = employeeService;
  }

  [HttpGet]  
  public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
  {
    var employees = await _employeeService.GetAllEmployeesAsync();
    return Ok(employees);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Employee>> GetEmployeeById(int id)
  {
    var employee = await _employeeService.GetEmployeeByIdAsync(id);
    if(employee == null) return NotFound();

    return Ok(employee);
  }

  [HttpPost]
  public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
  {
    if(!ModelState.IsValid) return BadRequest(ModelState);

    var createdEmployee = await _employeeService.CreateEmployeeAsync(employee);
    return CreatedAtAction(nameof(GetEmployeeById), new{id = createdEmployee.Id}, createdEmployee);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<Employee>> UpdateEmployee(int id, [FromBody] Employee employee)
  {
    if(!ModelState.IsValid) return BadRequest(ModelState);

    var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, employee);

    if(updatedEmployee == null) return NotFound();

    return Ok(updatedEmployee);
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteEmployee(int id)
  {
    var deleted = await _employeeService.DeleteEmployeeAsync(id);
    if(!deleted) return NotFound();

    return NoContent();
  }
}