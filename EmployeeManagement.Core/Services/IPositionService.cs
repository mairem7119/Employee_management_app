using EmployeeManagement.Core.Entities;
namespace EmployeeManagement.Core.Services;

public interface IPositionService
{
  Task<IEnumerable<Position>> GetAllPositionsAsync();
  Task<Position?> GetPositionByIdAsync(int id);
  Task<Position> CreatePositionAsync(Position position);
  Task<Position?> UpdatePositionAsync(int id, Position position);
  Task<bool> DeletePositionAsync(int id);
}