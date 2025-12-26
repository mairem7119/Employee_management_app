using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return View(employees);
    }

    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        return View(employee);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Convert HireDate sang UTC nếu cần
                if (employee.HireDate.Kind != DateTimeKind.Utc)
                {
                    employee.HireDate = DateTime.SpecifyKind(employee.HireDate.Date, DateTimeKind.Utc);
                }
                
                await _employeeService.CreateEmployeeAsync(employee);
                TempData["SuccessMessage"] = "Nhân viên đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException ex)
        {
            var innerEx = ex.InnerException;
            var errorMessage = innerEx?.Message ?? ex.Message;
            
            if (errorMessage.Contains("IX_Employees_Email") || 
                errorMessage.Contains("duplicate key value") ||
                errorMessage.Contains("unique constraint"))
            {
                ModelState.AddModelError("Email", "Email này đã tồn tại. Vui lòng sử dụng email khác.");
            }
            else if (errorMessage.Contains("timestamp with time zone") || 
                    errorMessage.Contains("DateTime with Kind"))
            {
                ModelState.AddModelError("HireDate", "Lỗi xử lý ngày tháng. Vui lòng thử lại.");
            }
            else if (errorMessage.Contains("not null") || 
                    errorMessage.Contains("null value"))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin bắt buộc.");
            }
            else
            {
                ModelState.AddModelError("", $"Lỗi database: {errorMessage}");
            }
            
            Console.WriteLine($"DbUpdateException: {ex.Message}");
            if (innerEx != null)
            {
                Console.WriteLine($"Inner Exception: {innerEx.Message}");
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", $"Lỗi: {ex.Message}");
            Console.WriteLine($"InvalidOperationException: {ex.Message}");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Lỗi không xác định: {ex.Message}");
            Console.WriteLine($"Exception: {ex.Message}");
        }

        return View(employee);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id)
            return NotFound();

        try
        {
            if (ModelState.IsValid)
            {
                // Convert HireDate sang UTC nếu cần
                if (employee.HireDate.Kind != DateTimeKind.Utc)
                {
                    employee.HireDate = DateTime.SpecifyKind(employee.HireDate.Date, DateTimeKind.Utc);
                }
                
                var result = await _employeeService.UpdateEmployeeAsync(id, employee);
                if (result == null)
                    return NotFound();

                TempData["SuccessMessage"] = "Thông tin nhân viên đã được cập nhật!";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException ex)
        {
            var innerEx = ex.InnerException;
            var errorMessage = innerEx?.Message ?? ex.Message;
            
            if (errorMessage.Contains("IX_Employees_Email") || 
                errorMessage.Contains("duplicate key value"))
            {
                ModelState.AddModelError("Email", "Email này đã tồn tại. Vui lòng sử dụng email khác.");
            }
            else if (errorMessage.Contains("timestamp with time zone") || 
                    errorMessage.Contains("DateTime with Kind"))
            {
                ModelState.AddModelError("HireDate", "Lỗi xử lý ngày tháng. Vui lòng thử lại.");
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

        return View(employee);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _employeeService.DeleteEmployeeAsync(id);
            TempData["SuccessMessage"] = "Nhân viên đã được xóa thành công!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}