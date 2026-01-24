# Hướng dẫn thêm các tính năng cho Website Quản lý Nhân viên

File này chứa code hướng dẫn để bạn tự tay thêm các tính năng vào hệ thống.

---

## 1. SỬA TRANG HOME - HIỂN THỊ THỐNG KÊ CHÍNH XÁC

### Bước 1: Sửa HomeController.cs

**File:** `EmployeeManagement.Web/Controllers/HomeController.cs`

```csharp
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Web.Models;
using EmployeeManagement.Core.Services;  // Thêm dòng này

namespace EmployeeManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;

    public HomeController(IEmployeeService employeeService, IDepartmentService departmentService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        var departments = await _departmentService.GetAllDepartmentsAsync();
        
        ViewBag.TotalEmployees = employees.Count();
        ViewBag.TotalDepartments = departments.Count();
        
        return View();
    }

    // ... các method khác giữ nguyên
}
```

### Bước 2: Sửa View Home/Index.cshtml

**File:** `EmployeeManagement.Web/Views/Home/Index.cshtml`

```razor
@{
    ViewData["Title"] = "Trang chủ";
}

<div class="text-center">
    <h1 class="display-4">Hệ thống Quản lý Nhân viên</h1>
    <p class="lead">Chào mừng đến với hệ thống quản lý nhân viên công ty</p>
    <hr />
    <div class="mt-4">
        <div class="row justify-content-center mb-4">
            <div class="col-md-4 mb-3">
                <div class="card text-bg-primary">
                    <div class="card-body">
                        <h5 class="card-title">Tổng số nhân viên</h5>
                        <h2 class="display-4">@ViewBag.TotalEmployees</h2>
                    </div>
                </div>
            </div>
            <div class="col-md-4 mb-3">
                <div class="card text-bg-info">
                    <div class="card-body">
                        <h5 class="card-title">Tổng số phòng ban</h5>
                        <h2 class="display-4">@ViewBag.TotalDepartments</h2>
                    </div>
                </div>
            </div>
        </div>
        <div class="mt-4">
            <a asp-controller="Employees" asp-action="Index" class="btn btn-primary btn-lg me-2">
                <i class="bi bi-people"></i> Quản lý nhân viên
            </a>
            <a asp-controller="Departments" asp-action="Index" class="btn btn-primary btn-lg">
                <i class="bi bi-building"></i> Quản lý phòng ban
            </a>
        </div>
    </div>
</div>
```

---

## 2. THÊM TÍNH NĂNG TÌM KIẾM VÀ LỌC

### Bước 1: Thêm method SearchAsync vào IEmployeeRepository

**File:** `EmployeeManagement.Core/interfaces/IEmployeeRepository.cs`

```csharp
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
  
  // Thêm method mới này
  Task<IEnumerable<Employee>> SearchAsync(string? searchTerm, int? departmentId, string? position);
}
```

### Bước 2: Implement SearchAsync trong EmployeeRepository

**File:** `EmployeeManagement.Infrastructure/Repositories/EmployeeRepository.cs`

Thêm method này vào cuối class (trước dấu `}` cuối cùng):

```csharp
public async Task<IEnumerable<Employee>> SearchAsync(string? searchTerm, int? departmentId, string? position)
{
  var query = _context.Employees.Include(e => e.Department).AsQueryable();

  if (!string.IsNullOrWhiteSpace(searchTerm))
  {
    query = query.Where(e => 
      e.FirstName.Contains(searchTerm) ||
      e.LastName.Contains(searchTerm) ||
      e.Email.Contains(searchTerm) ||
      e.PhoneNumber.Contains(searchTerm));
  }

  if (departmentId.HasValue && departmentId.Value > 0)
  {
    query = query.Where(e => e.DepartmentId == departmentId.Value);
  }

  if (!string.IsNullOrWhiteSpace(position))
  {
    query = query.Where(e => e.Position.Contains(position));
  }

  return await query.AsNoTracking().ToListAsync();
}
```

### Bước 3: Thêm method vào IEmployeeService

**File:** `EmployeeManagement.Core/Services/IEmployeeService.cs`

```csharp
using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Core.Services;

public interface IEmployeeService
{
  Task<IEnumerable<Employee>> GetAllEmployeesAsync();
  Task<Employee?> GetEmployeeByIdAsync(int id); 
  Task<Employee> CreateEmployeeAsync(Employee employee);
  Task<Employee?> UpdateEmployeeAsync(int id, Employee employee);  
  Task<bool> DeleteEmployeeAsync(int id);
  
  // Thêm method mới
  Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm, int? departmentId, string? position);
}
```

### Bước 4: Implement trong EmployeeService

**File:** `EmployeeManagement.Core/Services/EmployeeService.cs`

Thêm method này vào cuối class:

```csharp
public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm, int? departmentId, string? position)
{
  return await _employeeRepository.SearchAsync(searchTerm, departmentId, position);
}
```

### Bước 5: Cập nhật EmployeesController

**File:** `EmployeeManagement.Web/Controllers/EmployeesController.cs`

Sửa method `Index`:

```csharp
public async Task<IActionResult> Index(string searchTerm, int? departmentId, string position)
{
    IEnumerable<Employee> employees;

    if (!string.IsNullOrWhiteSpace(searchTerm) || departmentId.HasValue || !string.IsNullOrWhiteSpace(position))
    {
        employees = await _employeeService.SearchEmployeesAsync(searchTerm, departmentId, position);
    }
    else
    {
        employees = await _employeeService.GetAllEmployeesAsync();
    }

    ViewBag.Departments = await _departmentRepository.GetAllAsync();
    ViewBag.SearchTerm = searchTerm;
    ViewBag.SelectedDepartmentId = departmentId;
    ViewBag.SelectedPosition = position;

    return View(employees);
}
```

### Bước 6: Thêm form tìm kiếm vào View

**File:** `EmployeeManagement.Web/Views/Employees/Index.cshtml`

Thêm form tìm kiếm sau dòng `<h2>Danh sách nhân viên</h2>` và trước `@if (Model != null && Model.Any())`:

```razor
<!-- Form tìm kiếm -->
<div class="card mb-4">
    <div class="card-header">
        <h5 class="mb-0"><i class="bi bi-search"></i> Tìm kiếm và Lọc</h5>
    </div>
    <div class="card-body">
        <form asp-action="Index" method="get" class="row g-3">
            <div class="col-md-4">
                <label for="searchTerm" class="form-label">Tìm kiếm</label>
                <input type="text" class="form-control" id="searchTerm" name="searchTerm" 
                       value="@ViewBag.SearchTerm" placeholder="Tên, email, số điện thoại...">
            </div>
            <div class="col-md-3">
                <label for="departmentId" class="form-label">Phòng ban</label>
                <select class="form-select" id="departmentId" name="departmentId">
                    <option value="">Tất cả phòng ban</option>
                    @foreach (var dept in ViewBag.Departments as IEnumerable<EmployeeManagement.Core.Entities.Department>)
                    {
                        <option value="@dept.Id" selected="@(ViewBag.SelectedDepartmentId != null && ViewBag.SelectedDepartmentId == dept.Id)">
                            @dept.Name
                        </option>
                    }
                </select>
            </div>
            <div class="col-md-3">
                <label for="position" class="form-label">Chức vụ</label>
                <input type="text" class="form-control" id="position" name="position" 
                       value="@ViewBag.SelectedPosition" placeholder="Nhập chức vụ...">
            </div>
            <div class="col-md-2 d-flex align-items-end">
                <button type="submit" class="btn btn-primary w-100">
                    <i class="bi bi-search"></i> Tìm kiếm
                </button>
            </div>
            @if (!string.IsNullOrWhiteSpace(ViewBag.SearchTerm) || ViewBag.SelectedDepartmentId != null || !string.IsNullOrWhiteSpace(ViewBag.SelectedPosition))
            {
                <div class="col-12">
                    <a asp-action="Index" class="btn btn-secondary">
                        <i class="bi bi-x-circle"></i> Xóa bộ lọc
                    </a>
                </div>
            }
        </form>
    </div>
</div>
```

Cập nhật phần `else` để hiển thị thông báo phù hợp:

```razor
else
{
    <div class="alert alert-info" role="alert">
        @if (!string.IsNullOrWhiteSpace(ViewBag.SearchTerm) || ViewBag.SelectedDepartmentId != null || !string.IsNullOrWhiteSpace(ViewBag.SelectedPosition))
        {
            <text>Không tìm thấy nhân viên nào phù hợp với tiêu chí tìm kiếm. <a asp-action="Index">Xem tất cả nhân viên</a></text>
        }
        else
        {
            <text>Chưa có nhân viên nào. <a asp-action="Create">Thêm nhân viên đầu tiên</a></text>
        }
    </div>
}
```

---

## 3. THÊM PHÂN TRANG (PAGINATION)

### Bước 1: Tạo class PagedResult

**File mới:** `EmployeeManagement.Core/Models/PagedResult.cs`

```csharp
namespace EmployeeManagement.Core.Models;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

### Bước 2: Thêm method GetPagedAsync vào IEmployeeRepository

**File:** `EmployeeManagement.Core/interfaces/IEmployeeRepository.cs`

Thêm vào interface:

```csharp
using EmployeeManagement.Core.Models;  // Thêm using này

// Thêm method mới
Task<PagedResult<Employee>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, int? departmentId, string? position, string? sortBy, string? sortOrder);
```

### Bước 3: Implement GetPagedAsync trong EmployeeRepository

**File:** `EmployeeManagement.Infrastructure/Repositories/EmployeeRepository.cs`

Thêm using:
```csharp
using EmployeeManagement.Core.Models;
```

Thêm method (rất dài, copy toàn bộ):

```csharp
public async Task<PagedResult<Employee>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, int? departmentId, string? position, string? sortBy, string? sortOrder)
{
  var query = _context.Employees.Include(e => e.Department).AsQueryable();

  // Apply filters
  if (!string.IsNullOrWhiteSpace(searchTerm))
  {
    query = query.Where(e => 
      e.FirstName.Contains(searchTerm) ||
      e.LastName.Contains(searchTerm) ||
      e.Email.Contains(searchTerm) ||
      e.PhoneNumber.Contains(searchTerm));
  }

  if (departmentId.HasValue && departmentId.Value > 0)
  {
    query = query.Where(e => e.DepartmentId == departmentId.Value);
  }

  if (!string.IsNullOrWhiteSpace(position))
  {
    query = query.Where(e => e.Position.Contains(position));
  }

  // Apply sorting
  sortBy = sortBy?.ToLower() ?? "id";
  sortOrder = sortOrder?.ToLower() ?? "asc";

  query = sortBy switch
  {
    "name" or "firstname" => sortOrder == "desc" 
      ? query.OrderByDescending(e => e.FirstName).ThenByDescending(e => e.LastName)
      : query.OrderBy(e => e.FirstName).ThenBy(e => e.LastName),
    "email" => sortOrder == "desc" 
      ? query.OrderByDescending(e => e.Email)
      : query.OrderBy(e => e.Email),
    "department" => sortOrder == "desc" 
      ? query.OrderByDescending(e => e.Department != null ? e.Department.Name : "")
      : query.OrderBy(e => e.Department != null ? e.Department.Name : ""),
    "position" => sortOrder == "desc" 
      ? query.OrderByDescending(e => e.Position)
      : query.OrderBy(e => e.Position),
    "salary" => sortOrder == "desc" 
      ? query.OrderByDescending(e => e.Salary)
      : query.OrderBy(e => e.Salary),
    "hiredate" => sortOrder == "desc" 
      ? query.OrderByDescending(e => e.HireDate)
      : query.OrderBy(e => e.HireDate),
    _ => sortOrder == "desc" 
      ? query.OrderByDescending(e => e.Id)
      : query.OrderBy(e => e.Id)
  };

  var totalCount = await query.CountAsync();
  var items = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .AsNoTracking()
    .ToListAsync();

  return new PagedResult<Employee>
  {
    Items = items,
    TotalCount = totalCount,
    PageNumber = pageNumber,
    PageSize = pageSize
  };
}
```

### Bước 4: Thêm method vào IEmployeeService

**File:** `EmployeeManagement.Core/Services/IEmployeeService.cs`

```csharp
using EmployeeManagement.Core.Models;  // Thêm using

// Thêm method
Task<PagedResult<Employee>> GetPagedEmployeesAsync(int pageNumber, int pageSize, string? searchTerm, int? departmentId, string? position, string? sortBy, string? sortOrder);
```

### Bước 5: Implement trong EmployeeService

**File:** `EmployeeManagement.Core/Services/EmployeeService.cs`

```csharp
using EmployeeManagement.Core.Models;  // Thêm using

// Thêm method
public async Task<PagedResult<Employee>> GetPagedEmployeesAsync(int pageNumber, int pageSize, string? searchTerm, int? departmentId, string? position, string? sortBy, string? sortOrder)
{
  return await _employeeRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, departmentId, position, sortBy, sortOrder);
}
```

### Bước 6: Cập nhật EmployeesController

**File:** `EmployeeManagement.Web/Controllers/EmployeesController.cs`

Sửa method `Index`:

```csharp
public async Task<IActionResult> Index(string searchTerm, int? departmentId, string position, int page = 1, int pageSize = 10, string sortBy = "id", string sortOrder = "asc")
{
    var pagedResult = await _employeeService.GetPagedEmployeesAsync(page, pageSize, searchTerm, departmentId, position, sortBy, sortOrder);

    ViewBag.Departments = await _departmentRepository.GetAllAsync();
    ViewBag.SearchTerm = searchTerm;
    ViewBag.SelectedDepartmentId = departmentId;
    ViewBag.SelectedPosition = position;
    ViewBag.CurrentPage = page;
    ViewBag.PageSize = pageSize;
    ViewBag.SortBy = sortBy;
    ViewBag.SortOrder = sortOrder;

    return View(pagedResult);
}
```

### Bước 7: Cập nhật View

**File:** `EmployeeManagement.Web/Views/Employees/Index.cshtml`

Thay đổi dòng đầu:
```razor
@model EmployeeManagement.Core.Models.PagedResult<EmployeeManagement.Core.Entities.Employee>
```

Thay đổi `@if (Model != null && Model.Any())` thành:
```razor
@if (Model != null && Model.Items.Any())
```

Thay đổi `@foreach (var employee in Model)` thành:
```razor
@foreach (var employee in Model.Items)
```

Thêm pagination sau bảng (trước `</div>` cuối cùng):

```razor
<!-- Pagination -->
@if (Model.TotalPages > 1)
{
    <nav aria-label="Page navigation">
        <ul class="pagination justify-content-center">
            <li class="page-item @(Model.HasPreviousPage ? "" : "disabled")">
                <a class="page-link" asp-action="Index" 
                   asp-route-page="@(Model.PageNumber - 1)"
                   asp-route-searchTerm="@ViewBag.SearchTerm" 
                   asp-route-departmentId="@ViewBag.SelectedDepartmentId" 
                   asp-route-position="@ViewBag.SelectedPosition"
                   asp-route-sortBy="@ViewBag.SortBy"
                   asp-route-sortOrder="@ViewBag.SortOrder">Trước</a>
            </li>
            @for (int i = 1; i <= Model.TotalPages; i++)
            {
                <li class="page-item @(i == Model.PageNumber ? "active" : "")">
                    <a class="page-link" asp-action="Index" 
                       asp-route-page="@i"
                       asp-route-searchTerm="@ViewBag.SearchTerm" 
                       asp-route-departmentId="@ViewBag.SelectedDepartmentId" 
                       asp-route-position="@ViewBag.SelectedPosition"
                       asp-route-sortBy="@ViewBag.SortBy"
                       asp-route-sortOrder="@ViewBag.SortOrder">@i</a>
                </li>
            }
            <li class="page-item @(Model.HasNextPage ? "" : "disabled")">
                <a class="page-link" asp-action="Index" 
                   asp-route-page="@(Model.PageNumber + 1)"
                   asp-route-searchTerm="@ViewBag.SearchTerm" 
                   asp-route-departmentId="@ViewBag.SelectedDepartmentId" 
                   asp-route-position="@ViewBag.SelectedPosition"
                   asp-route-sortBy="@ViewBag.SortBy"
                   asp-route-sortOrder="@ViewBag.SortOrder">Sau</a>
            </li>
        </ul>
    </nav>
    <div class="text-center mt-2">
        <small class="text-muted">
            Hiển thị @((Model.PageNumber - 1) * Model.PageSize + 1) - @(Math.Min(Model.PageNumber * Model.PageSize, Model.TotalCount)) 
            trong tổng số @Model.TotalCount nhân viên
        </small>
    </div>
}
```

Cập nhật form tìm kiếm, thêm hidden fields:

```razor
<form asp-action="Index" method="get" class="row g-3">
    <input type="hidden" name="page" value="1" />
    <input type="hidden" name="sortBy" value="@ViewBag.SortBy" />
    <input type="hidden" name="sortOrder" value="@ViewBag.SortOrder" />
    <!-- ... các field khác giữ nguyên ... -->
</form>
```

---

## 4. THÊM SẮP XẾP (SORTING)

Tính năng sorting đã được tích hợp trong method `GetPagedAsync` ở phần 3. Bây giờ chỉ cần thêm UI vào View.

**File:** `EmployeeManagement.Web/Views/Employees/Index.cshtml`

Thay đổi các header của bảng thành link có thể click:

```razor
<thead class="table-dark">
    <tr>
        <th>
            <a asp-action="Index" asp-route-sortBy="id" asp-route-sortOrder="@(ViewBag.SortBy == "id" && ViewBag.SortOrder == "asc" ? "desc" : "asc")" 
               asp-route-searchTerm="@ViewBag.SearchTerm" asp-route-departmentId="@ViewBag.SelectedDepartmentId" asp-route-position="@ViewBag.SelectedPosition"
               class="text-white text-decoration-none">
                Mã nhân viên
                @if (ViewBag.SortBy == "id")
                {
                    <i class="bi bi-arrow-@(ViewBag.SortOrder == "asc" ? "up" : "down")"></i>
                }
            </a>
        </th>
        <th>
            <a asp-action="Index" asp-route-sortBy="name" asp-route-sortOrder="@(ViewBag.SortBy == "name" && ViewBag.SortOrder == "asc" ? "desc" : "asc")"
               asp-route-searchTerm="@ViewBag.SearchTerm" asp-route-departmentId="@ViewBag.SelectedDepartmentId" asp-route-position="@ViewBag.SelectedPosition"
               class="text-white text-decoration-none">
                Họ và tên
                @if (ViewBag.SortBy == "name")
                {
                    <i class="bi bi-arrow-@(ViewBag.SortOrder == "asc" ? "up" : "down")"></i>
                }
            </a>
        </th>
        <!-- Tương tự cho các cột khác: email, department, position, salary, hiredate -->
    </tr>
</thead>
```

---

## 5. THÊM SWAGGER CHO API

**File:** `EmployeeManagement.Api/Program.cs`

Thêm sau `builder.Services.AddControllers();`:

```csharp
// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Employee Management API",
        Version = "v1",
        Description = "API để quản lý nhân viên và phòng ban",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Employee Management",
            Email = "support@employeemanagement.com"
        }
    });
});
```

Thêm vào pipeline (sau `var app = builder.Build();`):

```csharp
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API v1");
        c.RoutePrefix = string.Empty; // Swagger UI at root
    });
}
```

**Lưu ý:** Đảm bảo package `Swashbuckle.AspNetCore` đã được cài đặt trong `.csproj`.

---

## 6. THÊM VALIDATION ATTRIBUTES

### Employee Model

**File:** `EmployeeManagement.Core/Entities/Employee.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Core.Entities;

public class Employee
{
  public int Id { get; set; }
  
  [Required(ErrorMessage = "Họ là bắt buộc")]
  [StringLength(100, ErrorMessage = "Họ không được vượt quá 100 ký tự")]
  [Display(Name = "Họ")]
  public string FirstName { get; set; } = string.Empty;
  
  [Required(ErrorMessage = "Tên là bắt buộc")]
  [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
  [Display(Name = "Tên")]
  public string LastName { get; set; } = string.Empty;
  
  [Required(ErrorMessage = "Email là bắt buộc")]
  [EmailAddress(ErrorMessage = "Email không hợp lệ")]
  [StringLength(200, ErrorMessage = "Email không được vượt quá 200 ký tự")]
  [Display(Name = "Email")]
  public string Email { get; set; } = string.Empty;
  
  [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
  [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Số điện thoại phải có 10-11 chữ số")]
  [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
  [Display(Name = "Số điện thoại")]
  public string PhoneNumber { get; set; } = string.Empty;
  
  [Required(ErrorMessage = "Phòng ban là bắt buộc")]
  [Display(Name = "Phòng ban")]
  public int DepartmentId { get; set; }  
  public Department? Department { get; set; }  
  
  [Required(ErrorMessage = "Chức vụ là bắt buộc")]
  [StringLength(100, ErrorMessage = "Chức vụ không được vượt quá 100 ký tự")]
  [Display(Name = "Chức vụ")]
  public string Position { get; set; } = string.Empty;
  
  [Required(ErrorMessage = "Lương là bắt buộc")]
  [Range(0, double.MaxValue, ErrorMessage = "Lương phải lớn hơn hoặc bằng 0")]
  [Display(Name = "Lương")]
  public decimal Salary { get; set; } 
  
  [Required(ErrorMessage = "Ngày vào làm là bắt buộc")]
  [DataType(DataType.Date)]
  [Display(Name = "Ngày vào làm")]
  public DateTime HireDate { get; set; }  
  public DateTime? CreatedAt { get; set; }  
  public DateTime? UpdatedAt { get; set; }  
}
```

### Department Model

**File:** `EmployeeManagement.Core/Entities/Department.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Core.Entities;

public class Department 
{
  public int Id { get; set; }
  
  [Required(ErrorMessage = "Mã phòng ban là bắt buộc")]
  [StringLength(10, MinimumLength = 2, ErrorMessage = "Mã phòng ban phải có từ 2 đến 10 ký tự")]
  [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Mã phòng ban chỉ được chứa chữ cái in hoa và số")]
  [Display(Name = "Mã phòng ban")]
  public string Code {get; set;} = string.Empty;
  
  [Required(ErrorMessage = "Tên phòng ban là bắt buộc")]
  [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên phòng ban phải có từ 2 đến 100 ký tự")]
  [Display(Name = "Tên phòng ban")]
  public string Name {get; set;} = string.Empty;
  
  [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
  [Display(Name = "Mô tả")]
  public string Description {get; set;} = string.Empty;
  
  [Required(ErrorMessage = "Trạng thái là bắt buộc")]
  [Display(Name = "Trạng thái")]
  public string IsActive {get; set;} = string.Empty;
  public DateTime CreatedAt {get; set;}
  public DateTime UpdatedAt {get; set;}
}
```

---

## LƯU Ý QUAN TRỌNG

1. **Thứ tự thực hiện:** Nên làm theo thứ tự từ 1 đến 6
2. **Build sau mỗi bước:** Chạy `dotnet build` sau mỗi bước để kiểm tra lỗi
3. **Test:** Test từng tính năng sau khi hoàn thành
4. **Backup:** Nên commit code vào git sau mỗi bước hoàn thành

---

## CÁC TÍNH NĂNG CÓ THỂ THÊM TIẾP

- Export Excel
- Authentication & Authorization
- Logging
- Unit Tests
- Error Handling Middleware
- Reports & Analytics
- Upload ảnh nhân viên
- Email notifications

Chúc bạn code vui vẻ! 🚀
