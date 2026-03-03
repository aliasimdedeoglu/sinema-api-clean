using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Exceptions;
namespace CinemaSystem.Domain.Entities;
public sealed class ShowTimeSeat : BaseEntity
{
    private ShowTimeSeat() { }
    public Guid ShowTimeId { get; private set; }
    public Guid SeatId { get; private set; }
    public bool IsReserved { get; private set; }
    public ShowTime ShowTime { get; private set; } = default!;
    public Seat Seat { get; private set; } = default!;
    public static ShowTimeSeat Create(Guid showTimeId, Guid seatId) =>
        new() { ShowTimeId = showTimeId, SeatId = seatId, IsReserved = false };
    public void Reserve()
    {
        if (IsReserved)
            throw new SeatAlreadyReservedException(SeatId);
        IsReserved = true;
    }
    public void Release()
    {
        IsReserved = false;
    }
}