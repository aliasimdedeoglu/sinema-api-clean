using CinemaSystem.Application.Features.Movies.Commands;
using CinemaSystem.Application.Features.Movies.Queries;
using CinemaSystem.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CinemaSystem.WebAPI.Controllers;
[Route("api/movies")]
public sealed class MoviesController(ISender mediator) : BaseApiController(mediator)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetMoviesQuery(page, pageSize), ct);
        return ToActionResult(result);
    }
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMovieById(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetMovieByIdQuery(id), ct);
        return ToActionResult(result);
    }
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> CreateMovie([FromBody] CreateMovieCommand command, CancellationToken ct = default)
    {
        var result = await Mediator.Send(command, ct);
        if (result.IsFailure)
            return ToActionResult(result);
        return CreatedAtAction(nameof(GetMovieById), new { id = result.Value!.Id }, result.Value);
    }
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteMovie(Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new DeleteMovieCommand(id), ct);
        return ToActionResult(result);
    }
}