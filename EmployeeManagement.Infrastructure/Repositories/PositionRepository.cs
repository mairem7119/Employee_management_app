using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Core.Entities;
using EmployeeManagement.Core.Interfaces;
using EmployeeManagement.Infrastructure.Data;

namespace EmployeeManagement.Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
  private readonly ApplicationDbContext _context;

  public PositionRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Position>> GetAllAsync()
  {
    return await _context.Positions.AsNoTracking().ToListAsync();
  }

  public async Task<Position?> GetByIdAsync(int id)
  {
    return await _context.Positions.FindAsync(id);
  }

  public async Task<Position> AddAsync(Position position)
  {
    try
    {
      position.CreatedAt = DateTime.UtcNow;
      position.UpdatedAt = DateTime.UtcNow;
      _context.Positions.Add(position);
      await _context.SaveChangesAsync();
      return position;
    }
    catch (DbUpdateException ex)
    {
      var innerException = ex.InnerException;
      if(innerException != null)
      {
        throw new InvalidOperationException(
          $"Database error: {innerException.Message}. " +
          $"Original error: {ex.Message}", ex);
      }
      throw;
    }
  }

  public async Task<Position> UpdateAsync(Position position)
  {
    var existing = await _context.Positions.FindAsync(position.Id);
    if(existing == null)
    {
      throw new InvalidOperationException("Position not found");
    }
    existing.Name = position.Name;
    existing.Description = position.Description;
    existing.UpdatedAt = DateTime.UtcNow;
    try
    {
      _context.Positions.Update(existing);
      await _context.SaveChangesAsync();
      return existing;
    }
    catch (DbUpdateException ex)
    {
      var innerException = ex.InnerException;
      if(innerException != null)
      {
        throw new InvalidOperationException(
          $"Database error: {innerException.Message}. " +
          $"Original error: {ex.Message}", ex);
      }
      throw;
    }
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var position = await _context.Positions.FindAsync(id);
    if(position == null)
    {
      return false;
    }
    _context.Positions.Remove(position);
    await _context.SaveChangesAsync();
    return true;
  }
}