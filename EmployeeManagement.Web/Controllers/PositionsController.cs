using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Services;
using EmployeeManagement.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Web.Controllers;

public class PositionsController : Controller
{
  private readonly IPositionService _positionService;

  public PositionsController(IPositionService positionService)
  {
    _positionService = positionService;
  }

  public async Task<IActionResult> Index(string? sortBy, string? sortOrder)
  {
    sortBy = string.IsNullOrWhiteSpace(sortBy) ? "Name" : sortBy.Trim();
    sortOrder = string.IsNullOrWhiteSpace(sortOrder) ? "asc" : sortOrder.Trim();
    if (sortOrder != "asc" && sortOrder != "desc") sortOrder = "asc";

    ViewBag.SortBy = sortBy;
    ViewBag.SortOrder = sortOrder;

    var positions = await _positionService.GetAllPositionsAsync(sortBy, sortOrder);
    return View(positions);
  }

  public async Task<IActionResult> Details(int id)
  {
    var position = await _positionService.GetPositionByIdAsync(id);
    if (position == null)
      return NotFound();
    return View(position);
  }

  public IActionResult Create()
  {
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(Position position)
  {
    try
    {
      if (ModelState.IsValid)
      {
        await _positionService.CreatePositionAsync(position);
        TempData["SuccessMessage"] = "Chức vụ đã được thêm thành công!";
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
        ModelState.AddModelError("Name", "Tên chức vụ này đã tồn tại. Vui lòng sử dụng tên khác.");
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

    return View(position);
  }

  public async Task<IActionResult> Edit(int id)
  {
    var position = await _positionService.GetPositionByIdAsync(id);
    if (position == null)
      return NotFound();

    return View(position);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, Position position)
  {
    if (id != position.Id)
      return NotFound();

    try
    {
      if (ModelState.IsValid)
      {
        var result = await _positionService.UpdatePositionAsync(id, position);
        if (result == null)
          return NotFound();

        TempData["SuccessMessage"] = "Thông tin chức vụ đã được cập nhật!";
        return RedirectToAction(nameof(Index));
      }
    }
    catch (DbUpdateException ex)
    {
      var innerEx = ex.InnerException;
      var errorMessage = innerEx?.Message ?? ex.Message;
      
      if (errorMessage.Contains("duplicate key value"))
      {
        ModelState.AddModelError("Name", "Tên chức vụ này đã tồn tại. Vui lòng sử dụng tên khác.");
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

    return View(position);
  }

  public async Task<IActionResult> Delete(int id)
  {
    var position = await _positionService.GetPositionByIdAsync(id);
    if (position == null)
      return NotFound();

    return View(position);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  [ActionName("Delete")]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    try
    {
      var deleted = await _positionService.DeletePositionAsync(id);
      if (!deleted)
      {
        TempData["ErrorMessage"] = "Không tìm thấy chức vụ để xóa.";
        return RedirectToAction(nameof(Index));
      }

      TempData["SuccessMessage"] = "Chức vụ đã được xóa thành công!";
    }
    catch (Exception ex)
    {
      TempData["ErrorMessage"] = $"Lỗi khi xóa: {ex.Message}";
    }

    return RedirectToAction(nameof(Index));
  }
}