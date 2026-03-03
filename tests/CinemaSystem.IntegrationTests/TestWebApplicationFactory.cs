using CinemaSystem.Application.Features.Reservations.Commands;
using CinemaSystem.Infrastructure;
using CinemaSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
namespace CinemaSystem.IntegrationTests;
public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"CinemaTest_{Guid.NewGuid()}";
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.AddScoped<IDbTransactionManager, NoOpDbTransactionManager>();
        });
    }
}
internal sealed class NoOpDbTransactionManager : IDbTransactionManager
{
    public Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct = default) =>
        Task.FromResult<IDbTransaction>(new NoOpDbTransaction());
}
internal sealed class NoOpDbTransaction : IDbTransaction
{
    public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task RollbackAsync(CancellationToken ct = default) => Task.CompletedTask;
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}