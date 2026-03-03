using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Exceptions;
using FluentAssertions;
namespace CinemaSystem.UnitTests.Domain;
public sealed class ShowTimeTests
{
    private static readonly Guid MovieId = Guid.NewGuid();
    private static readonly Guid HallId = Guid.NewGuid();
    private static readonly DateTime FutureBase = DateTime.UtcNow.AddDays(7);
    [Fact]
    public void Create_WhenHallIsEmpty_ShouldSucceed()
    {
        var act = () => ShowTime.Create(MovieId, HallId, FutureBase, 120, 15.00m, []);
        act.Should().NotThrow();
    }
    [Fact]
    public void Create_WhenOverlappingExistingShowTime_ShouldThrowShowTimeOverlapException()
    {
        var existing = ShowTime.Create(MovieId, HallId, FutureBase, 120, 15.00m, []);
        var act = () => ShowTime.Create(MovieId, HallId, FutureBase.AddMinutes(60), 90, 15.00m, [existing]);
        act.Should().Throw<ShowTimeOverlapException>()
            .WithMessage($"*{HallId}*");
    }
    [Fact]
    public void Create_WhenStartingExactlyAfterPreviousEnds_ShouldSucceed()
    {
        var showtimeA = ShowTime.Create(MovieId, HallId, FutureBase, 120, 15.00m, []);
        var showtimeAEnd = showtimeA.EndTime; // FutureBase + 135min
        var act = () => ShowTime.Create(MovieId, HallId, showtimeAEnd, 90, 15.00m, [showtimeA]);
        act.Should().NotThrow();
    }
    [Fact]
    public void Create_WhenScheduledInThePast_ShouldThrowArgumentException()
    {
        var act = () => ShowTime.Create(MovieId, HallId, DateTime.UtcNow.AddHours(-1), 90, 15.00m, []);
        act.Should().Throw<ArgumentException>();
    }
}