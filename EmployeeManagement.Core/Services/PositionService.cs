using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Interfaces;

namespace EmployeeManagement.Core.Services;

public class PositionService : IPositionService
{
  private readonly IPositionRepository _positionRepository;

  public PositionService(IPositionRepository positionRepository)
  {
    _positionRepository = positionRepository;
  }

  public async Task<IEnumerable<Position>> GetAllPositionsAsync()
  {
    return await _positionRepository.GetAllAsync();
  }

  public async Task<IEnumerable<Position>> GetAllPositionsAsync(string? sortBy, string? sortOrder)
  {
    var list = (await _positionRepository.GetAllAsync()).AsEnumerable();
    var orderAsc = string.IsNullOrEmpty(sortOrder) || sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);
    var by = (sortBy ?? "Name").Trim();

    return by switch
    {
      "Id" => orderAsc ? list.OrderBy(p => p.Id) : list.OrderByDescending(p => p.Id),
      "Name" => orderAsc ? list.OrderBy(p => p.Name) : list.OrderByDescending(p => p.Name),
      "CreatedAt" => orderAsc ? list.OrderBy(p => p.CreatedAt) : list.OrderByDescending(p => p.CreatedAt),
      "UpdatedAt" => orderAsc ? list.OrderBy(p => p.UpdatedAt) : list.OrderByDescending(p => p.UpdatedAt),
      _ => list.OrderBy(p => p.Name)
    };
  }

  public async Task<Position?> GetPositionByIdAsync(int id)
  {
    return await _positionRepository.GetByIdAsync(id);
  }

  public async Task<Position> CreatePositionAsync(Position position)
  {
    return await _positionRepository.AddAsync(position);
  }

  public async Task<Position?> UpdatePositionAsync(int id, Position position)
  {
    var existingPosition = await _positionRepository.GetByIdAsync(id);
    if(existingPosition == null)
    {
      return null;
    }
    existingPosition.Name = position.Name;
    existingPosition.Description = position.Description;
    existingPosition.UpdatedAt = DateTime.UtcNow;
    return await _positionRepository.UpdateAsync(existingPosition);
  }

  public async Task<bool> DeletePositionAsync(int id)
  {
    var existingPosition = await _positionRepository.GetByIdAsync(id);
    if(existingPosition == null)
    {
      return false;
    }
    return await _positionRepository.DeleteAsync(id);
  }
}