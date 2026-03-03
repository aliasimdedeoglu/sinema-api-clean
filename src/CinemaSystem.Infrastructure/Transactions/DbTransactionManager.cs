using CinemaSystem.Application.Features.Reservations.Commands;
using CinemaSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
namespace CinemaSystem.Infrastructure.Transactions;
public sealed class DbTransactionManager(CinemaDbContext context) : IDbTransactionManager
{
    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        var efTransaction = await context.Database.BeginTransactionAsync(ct);
        return new DbTransactionWrapper(efTransaction);
    }
}
internal sealed class DbTransactionWrapper(IDbContextTransaction efTransaction) : IDbTransaction
{
    public async Task CommitAsync(CancellationToken ct = default) =>
        await efTransaction.CommitAsync(ct);
    public async Task RollbackAsync(CancellationToken ct = default) =>
        await efTransaction.RollbackAsync(ct);
    public async ValueTask DisposeAsync() =>
        await efTransaction.DisposeAsync();
}