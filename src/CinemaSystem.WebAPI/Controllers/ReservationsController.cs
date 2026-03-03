using CinemaSystem.Application.Features.Reservations.Commands;
using CinemaSystem.Application.Features.Reservations.Queries;
using CinemaSystem.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CinemaSystem.WebAPI.Controllers;
[Route("api/reservations")]
[Authorize] // All reservation endpoints require authentication
public sealed class ReservationsController(ISender mediator) : BaseApiController(mediator)
{
    [HttpPost]
    [Authorize(Roles = Roles.Customer)]
    public async Task<IActionResult> ReserveSeats([FromBody] ReserveSeatsRequest request,
        CancellationToken ct = default)
    {
        var command = new ReserveSeatsCommand(CurrentUserId, request.ShowTimeId, request.SeatIds);
        var result = await Mediator.Send(command, ct);
        return ToActionResult(result);
    }
    [HttpGet("my")]
    public async Task<IActionResult> GetMyReservations([FromQuery] int page = 1,
        [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetMyReservationsQuery(CurrentUserId, page, pageSize), ct);
        return ToActionResult(result);
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReservationById(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetReservationByIdQuery(id, CurrentUserId, IsAdmin), ct);
        return ToActionResult(result);
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CancelReservation(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new CancelReservationCommand(id, CurrentUserId, IsAdmin), ct);
        return ToActionResult(result);
    }
}
public sealed record ReserveSeatsRequest(Guid ShowTimeId, IReadOnlyList<Guid> SeatIds);