using CinemaSystem.Domain.Entities;
namespace CinemaSystem.Domain.Repositories;
public interface IShowTimeRepository
{
    Task<ShowTime?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ShowTime>> GetByMovieIdAsync(Guid movieId, CancellationToken ct = default);
    Task<IReadOnlyList<ShowTime>> GetActiveByHallIdAsync(Guid hallId, CancellationToken ct = default);
    Task AddAsync(ShowTime showTime, CancellationToken ct = default);
    void Update(ShowTime showTime);
}