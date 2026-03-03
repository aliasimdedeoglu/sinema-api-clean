using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Enums;
using CinemaSystem.Domain.Events;
using CinemaSystem.Domain.Exceptions;
namespace CinemaSystem.Domain.Entities;
public sealed class Reservation : BaseEntity
{
    private Reservation() { }
    public Guid UserId { get; private set; }
    public Guid ShowTimeId { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal TotalPrice { get; private set; }
    public ShowTime ShowTime { get; private set; } = default!;
    private readonly List<ReservationSeat> _seats = [];
    public IReadOnlyCollection<ReservationSeat> Seats => _seats.AsReadOnly();
    public static Reservation Create(Guid userId, Guid showTimeId,
        IReadOnlyList<Guid> seatIds, decimal pricePerSeat)
    {
        if (seatIds.Count == 0)
            throw new EmptyReservationException();
        var reservation = new Reservation
        {
            UserId = userId,
            ShowTimeId = showTimeId,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            TotalPrice = pricePerSeat * seatIds.Count
        };
        foreach (var seatId in seatIds)
            reservation._seats.Add(ReservationSeat.Create(reservation.Id, seatId));
        reservation.RaiseDomainEvent(new ReservationCreatedEvent(reservation.Id, userId, seatIds));
        return reservation;
    }
    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
            throw new InvalidReservationStateException(Status.ToString(), "Confirmed");
        Status = ReservationStatus.Confirmed;
    }
    public void Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
            throw new InvalidReservationStateException(Status.ToString(), "Cancelled");
        Status = ReservationStatus.Cancelled;
        RaiseDomainEvent(new ReservationCancelledEvent(Id, UserId, _seats.Select(s => s.SeatId).ToList()));
    }
}