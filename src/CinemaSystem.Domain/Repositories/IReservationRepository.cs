using CinemaSystem.Domain.Entities;
namespace CinemaSystem.Domain.Repositories;
public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Reservation?> GetByIdWithSeatsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Reservation>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Reservation reservation, CancellationToken ct = default);
    void Update(Reservation reservation);
}