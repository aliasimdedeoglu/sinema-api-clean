using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Repositories;
using CinemaSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace CinemaSystem.Infrastructure.Repositories;
public sealed class MovieRepository(CinemaDbContext context) : IMovieRepository
{
    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Movies.FirstOrDefaultAsync(m => m.Id == id, ct);
    public async Task<IReadOnlyList<Movie>> GetAllAsync(int page, int pageSize, CancellationToken ct = default) =>
        await context.Movies
            .AsNoTracking()             // Read-only path: no change tracking overhead
            .OrderBy(m => m.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    public async Task<int> CountAsync(CancellationToken ct = default) =>
        await context.Movies.CountAsync(ct);
    public async Task AddAsync(Movie movie, CancellationToken ct = default) =>
        await context.Movies.AddAsync(movie, ct);
    public void Update(Movie movie) =>
        context.Movies.Update(movie);
}
public sealed class CinemaHallRepository(CinemaDbContext context) : ICinemaHallRepository
{
    public async Task<CinemaHall?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.CinemaHalls.FirstOrDefaultAsync(h => h.Id == id, ct);
    public async Task<CinemaHall?> GetByIdWithSeatsAsync(Guid id, CancellationToken ct = default) =>
        await context.CinemaHalls
            .Include(h => h.Seats)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    public async Task<IReadOnlyList<CinemaHall>> GetAllAsync(CancellationToken ct = default) =>
        await context.CinemaHalls.AsNoTracking().OrderBy(h => h.Name).ToListAsync(ct);
    public async Task AddAsync(CinemaHall hall, CancellationToken ct = default) =>
        await context.CinemaHalls.AddAsync(hall, ct);
    public void Update(CinemaHall hall) =>
        context.CinemaHalls.Update(hall);
}
public sealed class ShowTimeRepository(CinemaDbContext context) : IShowTimeRepository
{
    public async Task<ShowTime?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.ShowTimes
            .Include(st => st.Movie)
            .Include(st => st.Hall)
            .FirstOrDefaultAsync(st => st.Id == id, ct);
    public async Task<IReadOnlyList<ShowTime>> GetByMovieIdAsync(Guid movieId, CancellationToken ct = default) =>
        await context.ShowTimes
            .AsNoTracking()
            .Include(st => st.Movie)
            .Include(st => st.Hall)
            .Where(st => st.MovieId == movieId && st.StartTime > DateTime.UtcNow)
            .OrderBy(st => st.StartTime)
            .ToListAsync(ct);
    public async Task<IReadOnlyList<ShowTime>> GetActiveByHallIdAsync(Guid hallId, CancellationToken ct = default) =>
        await context.ShowTimes
            .Where(st => st.HallId == hallId && st.EndTime > DateTime.UtcNow)
            .ToListAsync(ct);
    public async Task AddAsync(ShowTime showTime, CancellationToken ct = default) =>
        await context.ShowTimes.AddAsync(showTime, ct);
    public void Update(ShowTime showTime) =>
        context.ShowTimes.Update(showTime);
}
public sealed class SeatRepository(CinemaDbContext context) : ISeatRepository
{
    public async Task<IReadOnlyList<Seat>> GetByIdsAsync(IEnumerable<Guid> seatIds, CancellationToken ct = default)
    {
        var ids = seatIds.ToList();
        return await context.Seats
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(ct);
    }
    public void UpdateRange(IEnumerable<Seat> seats) =>
        context.Seats.UpdateRange(seats);
}
public sealed class ReservationRepository(CinemaDbContext context) : IReservationRepository
{
    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Reservations.FirstOrDefaultAsync(r => r.Id == id, ct);
    public async Task<Reservation?> GetByIdWithSeatsAsync(Guid id, CancellationToken ct = default) =>
        await context.Reservations
            .Include(r => r.Seats)
                .ThenInclude(rs => rs.Seat)
            .Include(r => r.ShowTime)
                .ThenInclude(st => st.Movie)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    public async Task<IReadOnlyList<Reservation>> GetByUserIdAsync(Guid userId, int page, int pageSize,
        CancellationToken ct = default) =>
        await context.Reservations
            .AsNoTracking()
            .Include(r => r.Seats)
                .ThenInclude(rs => rs.Seat)
            .Include(r => r.ShowTime)
                .ThenInclude(st => st.Movie)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    public async Task AddAsync(Reservation reservation, CancellationToken ct = default) =>
        await context.Reservations.AddAsync(reservation, ct);
    public void Update(Reservation reservation) =>
        context.Reservations.Update(reservation);
}
public sealed class ShowTimeSeatRepository(CinemaDbContext context) : IShowTimeSeatRepository
{
    public async Task<IReadOnlyList<ShowTimeSeat>> GetByShowTimeAndSeatIdsAsync(
        Guid showTimeId, IEnumerable<Guid> seatIds, CancellationToken ct = default)
    {
        var ids = seatIds.ToList();
        return await context.ShowTimeSeats
            .Where(sts => sts.ShowTimeId == showTimeId && ids.Contains(sts.SeatId))
            .ToListAsync(ct);
    }
    public async Task<IReadOnlyList<ShowTimeSeat>> GetByShowTimeIdAsync(
        Guid showTimeId, CancellationToken ct = default) =>
        await context.ShowTimeSeats
            .AsNoTracking()
            .Include(sts => sts.Seat)
            .Where(sts => sts.ShowTimeId == showTimeId)
            .OrderBy(sts => sts.Seat.Row)
            .ThenBy(sts => sts.Seat.Number)
            .ToListAsync(ct);
    public async Task AddRangeAsync(IEnumerable<ShowTimeSeat> showTimeSeats, CancellationToken ct = default) =>
        await context.ShowTimeSeats.AddRangeAsync(showTimeSeats, ct);
    public void UpdateRange(IEnumerable<ShowTimeSeat> showTimeSeats) =>
        context.ShowTimeSeats.UpdateRange(showTimeSeats);
}