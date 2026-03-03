using CinemaSystem.Domain.Entities;
namespace CinemaSystem.Domain.Repositories;
public interface ISeatRepository
{
    Task<IReadOnlyList<Seat>> GetByIdsAsync(IEnumerable<Guid> seatIds, CancellationToken ct = default);
    void UpdateRange(IEnumerable<Seat> seats);
}