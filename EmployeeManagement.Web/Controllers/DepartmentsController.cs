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

  public async Task<IActionResult> Details(int id)
  {
    var department = await _departmentService.GetDepartmentByIdAsync(id);
    if (department == null)
      return NotFound();

    return View(department);
  }

  public IActionResult Create()
  {
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(Department department)
  {
    try
    {
      if (ModelState.IsValid)
      {
        await _departmentService.CreateDepartmentAsync(department);
        TempData["SuccessMessage"] = "Phòng ban đã được thêm thành công Rồi!";
        return RedirectToAction(nameof(Index));
      }
    }
    catch (DbUpdateException ex)
    {
      var innerEx = ex.InnerException;
      var errorMessage = innerEx?.Message ?? ex.Message;
      
      if (errorMessage.Contains("duplicate key value") || 
          errorMessage.Contains("unique constraint"))
      {
        ModelState.AddModelError("Code", "Mã phòng ban này đã tồn tại. Vui lòng sử dụng mã khác.");
      }
      else
      {
        ModelState.AddModelError("", $"Lỗi database: {errorMessage}");
      }
    }
    catch (Exception ex)
    {
      ModelState.AddModelError("", $"Lỗi: {ex.Message}");
    }

    return View(department);
  }

  public async Task<IActionResult> Edit(int id)
  {
    var department = await _departmentService.GetDepartmentByIdAsync(id);
    if (department == null)
      return NotFound();

    return View(department);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, Department department)
  {
    if (id != department.Id)
      return NotFound();

    try
    {
      if (ModelState.IsValid)
      {
        var result = await _departmentService.UpdateDepartmentAsync(id, department);
        if (result == null)
          return NotFound();

        TempData["SuccessMessage"] = "Thông tin phòng ban đã được cập nhật!";
        return RedirectToAction(nameof(Index));
      }
    }
    catch (DbUpdateException ex)
    {
      var innerEx = ex.InnerException;
      var errorMessage = innerEx?.Message ?? ex.Message;
      
      if (errorMessage.Contains("duplicate key value"))
      {
        ModelState.AddModelError("Code", "Mã phòng ban này đã tồn tại. Vui lòng sử dụng mã khác.");
      }
      else
      {
        ModelState.AddModelError("", $"Lỗi database: {errorMessage}");
      }
    }
    catch (Exception ex)
    {
      ModelState.AddModelError("", $"Lỗi: {ex.Message}");
    }

    return View(department);
  }

  public async Task<IActionResult> Delete(int id)
  {
    var department = await _departmentService.GetDepartmentByIdAsync(id);
    if (department == null)
      return NotFound();

    // Kiểm tra xem phòng ban có nhân viên không
    var employees = await _employeeRepository.GetAllAsync();
    var hasEmployees = employees.Any(e => e.DepartmentId == id);
    ViewBag.HasEmployees = hasEmployees;
    if (hasEmployees)
    {
      ViewBag.EmployeeCount = employees.Count(e => e.DepartmentId == id);
    }

    return View(department);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [ActionName("Delete")]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    try
    {
      // Kiểm tra xem phòng ban có nhân viên không
      var employees = await _employeeRepository.GetAllAsync();
      var hasEmployees = employees.Any(e => e.DepartmentId == id);
      
      if (hasEmployees)
      {
        TempData["ErrorMessage"] = "Không thể xóa phòng ban này vì còn nhân viên trong phòng ban. Vui lòng chuyển nhân viên sang phòng ban khác trước.";
        return RedirectToAction(nameof(Index));
      }

      var deleted = await _departmentService.DeleteDepartmentAsync(id);
      if (!deleted)
      {
        TempData["ErrorMessage"] = "Không tìm thấy phòng ban để xóa.";
        return RedirectToAction(nameof(Index));
      }

      TempData["SuccessMessage"] = "Phòng ban đã được xóa thành công!";
    }
    catch (Exception ex)
    {
      TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
    }

    return RedirectToAction(nameof(Index));
  }
}