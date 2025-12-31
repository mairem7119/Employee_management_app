using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Controllers;

public class DepartmentsController : Controller
{
  private readonly IDepartmentService _departmentService;
  private readonly IEmployeeRepository _employeeRepository;

  public DepartmentsController(IDepartmentService departmentService, IEmployeeRepository employeeRepository)
  {
    _departmentService = departmentService;
    _employeeRepository = employeeRepository;
  }

  public async Task<IActionResult> Index()
  {
    var departments = await _departmentService.GetAllDepartmentsAsync();
    return View(departments);
  }
}