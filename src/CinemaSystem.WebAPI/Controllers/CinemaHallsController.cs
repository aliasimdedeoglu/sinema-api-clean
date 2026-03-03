using CinemaSystem.Application.Features.Halls.Commands;
using CinemaSystem.Application.Features.Halls.Queries;
using CinemaSystem.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CinemaSystem.WebAPI.Controllers;
[Route("api/halls")]
public sealed class CinemaHallsController(ISender mediator) : BaseApiController(mediator)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetHalls(CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetHallsQuery(), ct);
        return ToActionResult(result);
    }
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHallById(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetHallByIdQuery(id), ct);
        return ToActionResult(result);
    }
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateHall([FromBody] CreateCinemaHallCommand command,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(command, ct);
        return ToActionResult(result);
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteHall(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new DeleteCinemaHallCommand(id), ct);
        return ToActionResult(result);
    }
}