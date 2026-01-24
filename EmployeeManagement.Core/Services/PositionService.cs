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