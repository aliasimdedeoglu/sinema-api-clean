using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Exceptions;
using FluentAssertions;
namespace CinemaSystem.UnitTests.Domain;
public sealed class ShowTimeSeatTests
{
    private static ShowTimeSeat CreateShowTimeSeat(bool isReserved = false)
    {
        var sts = (ShowTimeSeat)Activator.CreateInstance(typeof(ShowTimeSeat), true)!;
        typeof(ShowTimeSeat).GetProperty("Id")!.SetValue(sts, Guid.NewGuid());
        typeof(ShowTimeSeat).GetProperty("ShowTimeId")!.SetValue(sts, Guid.NewGuid());
        typeof(ShowTimeSeat).GetProperty("SeatId")!.SetValue(sts, Guid.NewGuid());
        typeof(ShowTimeSeat).GetProperty("IsReserved")!.SetValue(sts, isReserved);
        typeof(ShowTimeSeat).GetProperty("RowVersion")!.SetValue(sts, new byte[] { 0, 0, 0, 1 });
        return sts;
    }
    [Fact]
    public void Reserve_WhenSeatIsAvailable_ShouldMarkAsReserved()
    {
        var sts = CreateShowTimeSeat();
        sts.Reserve();
        sts.IsReserved.Should().BeTrue();
    }
    [Fact]
    public void Reserve_WhenAlreadyReserved_ShouldThrowSeatAlreadyReservedException()
    {
        var sts = CreateShowTimeSeat(isReserved: true);
        var act = () => sts.Reserve();
        act.Should().Throw<SeatAlreadyReservedException>()
            .WithMessage($"*{sts.SeatId}*");
    }
    [Fact]
    public void Release_WhenReserved_ShouldMarkAsAvailable()
    {
        var sts = CreateShowTimeSeat(isReserved: true);
        sts.Release();
        sts.IsReserved.Should().BeFalse();
    }
    [Fact]
    public void Release_ThenReserveAgain_ShouldSucceed()
    {
        var sts = CreateShowTimeSeat(isReserved: true);
        sts.Release();
        var act = () => sts.Reserve();
        act.Should().NotThrow();
        sts.IsReserved.Should().BeTrue();
    }
    [Fact]
    public void ShowTimeSeat_Create_ShouldHaveIsReservedFalse()
    {
        var hall = CinemaHall.Create("Test Hall");
        hall.ConfigureSeats([("A", 1)]);
        var seatId = hall.Seats.First().Id;
        var showTimeId = Guid.NewGuid();
        var sts = ShowTimeSeat.Create(showTimeId, seatId);
        sts.ShowTimeId.Should().Be(showTimeId);
        sts.SeatId.Should().Be(seatId);
        sts.IsReserved.Should().BeFalse();
    }
}