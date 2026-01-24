using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Core.Interfaces;

public interface IPositionRepository
{
  Task<IEnumerable<Position>> GetAllAsync();
  Task<Position?> GetByIdAsync(int id);
  Task<Position> AddAsync(Position position);
  Task<Position> UpdateAsync(Position position);
  Task<bool> DeleteAsync(int id);
}