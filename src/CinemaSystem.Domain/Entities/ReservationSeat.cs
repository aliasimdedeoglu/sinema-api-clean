namespace CinemaSystem.Domain.Entities;
public sealed class ReservationSeat
{
    private ReservationSeat() { }
    public Guid ReservationId { get; private set; }
    public Guid SeatId { get; private set; }
    public Reservation Reservation { get; private set; } = default!;
    public Seat Seat { get; private set; } = default!;
    internal static ReservationSeat Create(Guid reservationId, Guid seatId) =>
        new() { ReservationId = reservationId, SeatId = seatId };
}