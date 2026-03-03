using CinemaSystem.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace CinemaSystem.WebAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController(ISender mediator) : ControllerBase
{
    protected ISender Mediator { get; } = mediator;
    protected Guid CurrentUserId =>
        Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("User ID claim missing."));
    protected bool IsAdmin =>
        User.IsInRole("Admin");
    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);
        return result.StatusCode switch
        {
            404 => NotFound(new { result.Error }),
            409 => Conflict(new { result.Error }),
            403 => Forbid(),
            422 => UnprocessableEntity(new { result.Error }),
            _ => BadRequest(new { result.Error })
        };
    }
}