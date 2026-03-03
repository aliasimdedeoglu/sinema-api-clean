using CinemaSystem.Domain.Repositories;
using CinemaSystem.Infrastructure.Persistence;
namespace CinemaSystem.Infrastructure.Repositories;
public sealed class UnitOfWork(CinemaDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}