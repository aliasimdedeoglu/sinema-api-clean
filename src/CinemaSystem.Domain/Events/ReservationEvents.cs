using CinemaSystem.Domain.Common;
namespace CinemaSystem.Domain.Events;
public sealed record ReservationCreatedEvent(
    Guid ReservationId,
    Guid UserId,
    IReadOnlyList<Guid> SeatIds) : IDomainEvent;
public sealed record ReservationCancelledEvent(
    Guid ReservationId,
    Guid UserId,
    IReadOnlyList<Guid> SeatIds) : IDomainEvent;