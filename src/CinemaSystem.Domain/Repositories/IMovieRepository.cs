using CinemaSystem.Domain.Entities;
namespace CinemaSystem.Domain.Repositories;
public interface IMovieRepository
{
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Movie>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
    Task AddAsync(Movie movie, CancellationToken ct = default);
    void Update(Movie movie);
}