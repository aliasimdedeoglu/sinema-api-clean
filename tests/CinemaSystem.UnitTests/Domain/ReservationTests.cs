using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Enums;
using CinemaSystem.Domain.Exceptions;
using FluentAssertions;
namespace CinemaSystem.UnitTests.Domain;
public sealed class ReservationTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid ShowTimeId = Guid.NewGuid();
    [Fact]
    public void Create_WithValidSeats_ShouldBePendingStatus()
    {
        var seatIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var reservation = Reservation.Create(UserId, ShowTimeId, seatIds, 15.00m);
        reservation.Status.Should().Be(ReservationStatus.Pending);
        reservation.TotalPrice.Should().Be(30.00m); // 2 seats × 15.00
        reservation.Seats.Should().HaveCount(2);
        reservation.UserId.Should().Be(UserId);
    }
    [Fact]
    public void Create_WithNoSeats_ShouldThrowEmptyReservationException()
    {
        var act = () => Reservation.Create(UserId, ShowTimeId, [], 15.00m);
        act.Should().Throw<EmptyReservationException>();
    }
    [Fact]
    public void Confirm_WhenPending_ShouldTransitionToConfirmed()
    {
        var reservation = Reservation.Create(UserId, ShowTimeId, [Guid.NewGuid()], 15.00m);
        reservation.Confirm();
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
    }
    [Fact]
    public void Confirm_WhenAlreadyCancelled_ShouldThrowInvalidReservationStateException()
    {
        var reservation = Reservation.Create(UserId, ShowTimeId, [Guid.NewGuid()], 15.00m);
        reservation.Cancel();
        var act = () => reservation.Confirm();
        act.Should().Throw<InvalidReservationStateException>()
            .WithMessage("*Cancelled*Confirmed*");
    }
    [Fact]
    public void Cancel_WhenPending_ShouldTransitionToCancelled()
    {
        var reservation = Reservation.Create(UserId, ShowTimeId, [Guid.NewGuid()], 15.00m);
        reservation.Cancel();
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }
    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrowInvalidReservationStateException()
    {
        var reservation = Reservation.Create(UserId, ShowTimeId, [Guid.NewGuid()], 15.00m);
        reservation.Cancel();
        var act = () => reservation.Cancel();
        act.Should().Throw<InvalidReservationStateException>();
    }
    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        var seatIds = new List<Guid> { Guid.NewGuid() };
        var reservation = Reservation.Create(UserId, ShowTimeId, seatIds, 15.00m);
        reservation.DomainEvents.Should().HaveCount(1);
        reservation.DomainEvents.First().Should().BeOfType<CinemaSystem.Domain.Events.ReservationCreatedEvent>();
    }
}