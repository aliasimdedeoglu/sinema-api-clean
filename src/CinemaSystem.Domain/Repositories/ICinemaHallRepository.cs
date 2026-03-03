using CinemaSystem.Domain.Entities;
namespace CinemaSystem.Domain.Repositories;
public interface ICinemaHallRepository
{
    Task<CinemaHall?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CinemaHall>> GetAllAsync(CancellationToken ct = default);
    Task<CinemaHall?> GetByIdWithSeatsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(CinemaHall hall, CancellationToken ct = default);
    void Update(CinemaHall hall);
}