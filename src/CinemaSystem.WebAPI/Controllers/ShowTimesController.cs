using CinemaSystem.Application.Features.ShowTimes.Commands;
using CinemaSystem.Application.Features.ShowTimes.Queries;
using CinemaSystem.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CinemaSystem.WebAPI.Controllers;
[Route("api/showtimes")]
public sealed class ShowTimesController(ISender mediator) : BaseApiController(mediator)
{
    [HttpGet("movie/{movieId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetShowTimesByMovie(Guid movieId, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetShowTimesByMovieQuery(movieId), ct);
        return ToActionResult(result);
    }
    [HttpGet("{id:guid}/seats")]
    [AllowAnonymous]
    public async Task<IActionResult> GetShowTimeSeats(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetShowTimeSeatsQuery(id), ct);
        return ToActionResult(result);
    }
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateShowTime([FromBody] CreateShowTimeCommand command,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(command, ct);
        return ToActionResult(result);
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteShowTime(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new DeleteShowTimeCommand(id), ct);
        return ToActionResult(result);
    }
}