using CinemaSystem.Infrastructure.Authentication;
using CinemaSystem.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace CinemaSystem.WebAPI.Controllers;
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IJwtTokenService jwtTokenService,
    ISender mediator) : BaseApiController(mediator)
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));
        if (!await roleManager.RoleExistsAsync(Roles.Customer))
            await roleManager.CreateAsync(new ApplicationRole(Roles.Customer));
        await userManager.AddToRoleAsync(user, Roles.Customer);
        return Ok(new { message = "Registration successful." });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { message = "Invalid credentials." });
        var roles = await userManager.GetRolesAsync(user);
        var token = jwtTokenService.GenerateToken(user, roles);
        return Ok(new
        {
            token,
            userId = user.Id,
            email = user.Email,
            roles,
            expiresAt = DateTime.UtcNow.AddHours(24)
        });
    }
}
public sealed record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public sealed record LoginRequest(string Email, string Password);