namespace CinemaSystem.Application.Features.Reservations.Commands;
public interface IDbTransactionManager
{
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
public interface IDbTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}