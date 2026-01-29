using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Core.Interfaces;
using EmployeeManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPositionRepository  _positionRepository;

    public EmployeesController(IEmployeeService employeeService, IDepartmentRepository departmentRepository, IPositionRepository positionRepository)
    {   
        _employeeService = employeeService;
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
    }

    public async Task<IActionResult> Index(string? searchTerm, int? departmentId, string? position)
    {
        // Load danh sách phòng ban và chức vụ cho dropdown
        ViewBag.Departments = await _departmentRepository.GetAllAsync();
        ViewBag.Positions = await _positionRepository.GetAllAsync();
        
        // Lưu các giá trị filter vào ViewBag để giữ lại khi submit form
        ViewBag.SearchTerm = searchTerm;
        ViewBag.SelectedDepartmentId = departmentId;
        ViewBag.SelectedPosition = position;

        IEnumerable<Employee> employees;

        // Nếu có tham số tìm kiếm/lọc thì gọi SearchEmployeesAsync, ngược lại lấy tất cả
        if (!string.IsNullOrWhiteSpace(searchTerm) || 
            (departmentId.HasValue && departmentId.Value > 0) || 
            !string.IsNullOrWhiteSpace(position))
        {
            employees = await _employeeService.SearchEmployeesAsync(searchTerm, departmentId, position);
        }
        else
        {
            employees = await _employeeService.GetAllEmployeesAsync();
        }

        return View(employees);
    }

    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        return View(employee);
    }

    public async Task<IActionResult> Create()
    {

        var viewModel = new CreateEmployeeViewModel
        {
            Departments = await _departmentRepository.GetAllAsync() ?? new List<Department>(),
            Positions = await _positionRepository.GetAllAsync() ?? new List<Position>(),
        };

        return View(viewModel);
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

        var viewModel = new CreateEmployeeViewModel
        {
            Departments = await _departmentRepository.GetAllAsync() ?? new List<Department>(),
            Positions = await _positionRepository.GetAllAsync() ?? new List<Position>(),
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(id);
        if (employee == null)
            return NotFound();

        var departments = await _departmentRepository.GetAllAsync();
        var positions = await _positionRepository.GetAllAsync();

        var viewModel = new EditEmployeeViewModel
        {
            Employee = employee,
            Departments = departments ?? new List<Department>(),
            Positions = positions ?? new List<Position>()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditEmployeeViewModel viewModel)
    {
        if (viewModel == null || viewModel.Employee == null)
        {
            ModelState.AddModelError("", "Dữ liệu không hợp lệ.");
            viewModel = new EditEmployeeViewModel
            {
                Employee = new Employee(),
                Departments = await _departmentRepository.GetAllAsync() ?? new List<Department>(),
                Positions = await _positionRepository.GetAllAsync() ?? new List<Position>()
            };
            return View(viewModel);
        }

        if (id != viewModel.Employee.Id)
            return NotFound();

        try
        {
            if (ModelState.IsValid)
            {
                var employee = viewModel.Employee;
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
                ModelState.AddModelError("Employee.Email", "Email này đã tồn tại. Vui lòng sử dụng email khác.");
            }
            else if (errorMessage.Contains("timestamp with time zone") || 
                    errorMessage.Contains("DateTime with Kind"))
            {
                ModelState.AddModelError("Employee.HireDate", "Lỗi xử lý ngày tháng. Vui lòng thử lại.");
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

        // Load lại dữ liệu cho ViewModel khi có lỗi
        var departments = await _departmentRepository.GetAllAsync();
        var positions = await _positionRepository.GetAllAsync();
        
        viewModel.Departments = departments ?? new List<Department>();
        viewModel.Positions = positions ?? new List<Position>();
        
        return View(viewModel);
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