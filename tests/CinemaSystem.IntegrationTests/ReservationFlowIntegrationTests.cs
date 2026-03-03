using CinemaSystem.Application.Features.Halls.Commands;
using CinemaSystem.Application.Features.Movies.Commands;
using CinemaSystem.Application.Features.Movies.Queries;
using CinemaSystem.Application.Features.Reservations.Commands;
using CinemaSystem.Application.Features.ShowTimes.Commands;
using CinemaSystem.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
namespace CinemaSystem.IntegrationTests;
public sealed class ReservationFlowIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public ReservationFlowIntegrationTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task FullReservationFlow_ShouldSucceed()
    {
        using var scope = _factory.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var movieResult = await mediator.Send(new CreateMovieCommand(
            "Test Movie", "Description", 120,
            new DateOnly(2026, 6, 1), "Action"));
        movieResult.IsSuccess.Should().BeTrue();
        var movieId = movieResult.Value!.Id;
        var hallResult = await mediator.Send(new CreateCinemaHallCommand(
            "Integration Test Hall",
            [new SeatLayoutItem("A", 1), new SeatLayoutItem("A", 2), new SeatLayoutItem("A", 3)]));
        hallResult.IsSuccess.Should().BeTrue();
        var hallId = hallResult.Value!.Id;
        var showTimeResult = await mediator.Send(new CreateShowTimeCommand(
            movieId, hallId, DateTime.UtcNow.AddDays(3), 15.00m));
        showTimeResult.IsSuccess.Should().BeTrue();
        var showTimeId = showTimeResult.Value!.Id;
        var db = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
        var seats = await db.Seats.Where(s => s.HallId == hallId).ToListAsync();
        seats.Should().HaveCount(3);
        var seatToReserve = seats[0].Id;
        var userId = Guid.NewGuid();
        var reservationResult = await mediator.Send(
            new ReserveSeatsCommand(userId, showTimeId, [seatToReserve]));
        reservationResult.IsSuccess.Should().BeTrue();
        reservationResult.Value!.TotalPrice.Should().Be(15.00m);
        reservationResult.Value.Seats.Should().HaveCount(1);
        var showTimeSeat = await db.ShowTimeSeats
            .FirstOrDefaultAsync(sts => sts.ShowTimeId == showTimeId && sts.SeatId == seatToReserve);
        showTimeSeat.Should().NotBeNull();
        showTimeSeat!.IsReserved.Should().BeTrue();
    }
    [Fact]
    public async Task SimultaneousSeatReservation_SecondRequestShouldGetConflict()
    {
        using var scope1 = _factory.Services.CreateScope();
        using var scope2 = _factory.Services.CreateScope();
        var db1 = scope1.ServiceProvider.GetRequiredService<CinemaDbContext>();
        var db2 = scope2.ServiceProvider.GetRequiredService<CinemaDbContext>();
        var mediator1 = scope1.ServiceProvider.GetRequiredService<IMediator>();
        var movieResult = await mediator1.Send(new CreateMovieCommand(
            "Concurrent Movie", "Desc", 90, new DateOnly(2026, 7, 1), "Drama"));
        var hallResult = await mediator1.Send(new CreateCinemaHallCommand(
            "Concurrent Hall", [new SeatLayoutItem("B", 1)]));
        var showTimeResult = await mediator1.Send(new CreateShowTimeCommand(
            movieResult.Value!.Id, hallResult.Value!.Id, DateTime.UtcNow.AddDays(5), 20.00m));
        var seatId = (await db1.Seats.Where(s => s.HallId == hallResult.Value!.Id).FirstAsync()).Id;
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var showTimeId = showTimeResult.Value!.Id;
        var result1 = await mediator1.Send(new ReserveSeatsCommand(userId1, showTimeId, [seatId]));
        result1.IsSuccess.Should().BeTrue("First user should succeed");
        var mediator2 = scope2.ServiceProvider.GetRequiredService<IMediator>();
        var result2 = await mediator2.Send(new ReserveSeatsCommand(userId2, showTimeId, [seatId]));
        result2.IsFailure.Should().BeTrue("Second user must be rejected");
        result2.StatusCode.Should().BeOneOf(409, 409, 400); // Conflict or already reserved
    }
}