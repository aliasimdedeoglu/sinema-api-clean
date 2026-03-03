using CinemaSystem.Domain.Entities;
namespace CinemaSystem.Domain.Repositories;
public interface IShowTimeSeatRepository
{
    Task<IReadOnlyList<ShowTimeSeat>> GetByShowTimeAndSeatIdsAsync(
        Guid showTimeId, IEnumerable<Guid> seatIds, CancellationToken ct = default);
    Task<IReadOnlyList<ShowTimeSeat>> GetByShowTimeIdAsync(Guid showTimeId, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<ShowTimeSeat> showTimeSeats, CancellationToken ct = default);
    void UpdateRange(IEnumerable<ShowTimeSeat> showTimeSeats);
}