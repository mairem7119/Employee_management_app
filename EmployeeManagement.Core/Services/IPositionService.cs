using EmployeeManagement.Core.Entities;
namespace EmployeeManagement.Core.Services;

public interface IPositionService
{
  Task<IEnumerable<Position>> GetAllPositionsAsync();
  /// <summary>Lấy tất cả chức vụ với sắp xếp. sortBy: Name, Id, CreatedAt, UpdatedAt; sortOrder: asc hoặc desc.</summary>
  Task<IEnumerable<Position>> GetAllPositionsAsync(string? sortBy, string? sortOrder);
  Task<Position?> GetPositionByIdAsync(int id);
  Task<Position> CreatePositionAsync(Position position);
  Task<Position?> UpdatePositionAsync(int id, Position position);
  Task<bool> DeletePositionAsync(int id);
}