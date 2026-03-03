namespace CinemaSystem.Domain.Exceptions;
public sealed class SeatAlreadyReservedException(Guid seatId)
    : DomainException($"Seat '{seatId}' is already reserved.");
public sealed class SeatNotInHallException(Guid seatId, Guid hallId)
    : DomainException($"Seat '{seatId}' does not belong to hall '{hallId}'.");
public sealed class EmptyReservationException()
    : DomainException("A reservation must include at least one seat.");
public sealed class ShowTimeOverlapException(Guid hallId, DateTime start, DateTime end)
    : DomainException($"Hall '{hallId}' already has a showtime scheduled between {start:g} and {end:g}.");
public sealed class ShowTimeCapacityExceededException(int capacity)
    : DomainException($"Cannot reserve more seats than the hall capacity ({capacity}).");
public sealed class InvalidReservationStateException(string current, string attempted)
    : DomainException($"Cannot transition reservation from '{current}' to '{attempted}'.");