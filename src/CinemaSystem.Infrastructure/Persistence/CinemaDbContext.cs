using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace CinemaSystem.Infrastructure.Persistence;
public sealed class CinemaDbContext(
    DbContextOptions<CinemaDbContext> options,
    IMediator mediator) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<CinemaHall> CinemaHalls => Set<CinemaHall>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<ShowTime> ShowTimes => Set<ShowTime>();
    public DbSet<ShowTimeSeat> ShowTimeSeats => Set<ShowTimeSeat>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationSeat> ReservationSeats => Set<ReservationSeat>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Must call base for Identity tables
        builder.ApplyConfigurationsFromAssembly(typeof(CinemaDbContext).Assembly);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
        var result = await base.SaveChangesAsync(cancellationToken);
        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent, cancellationToken);
        ChangeTracker
            .Entries<BaseEntity>()
            .ToList()
            .ForEach(e => e.Entity.ClearDomainEvents());
        return result;
    }
}