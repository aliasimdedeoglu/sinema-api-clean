using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CinemaSystem.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
namespace CinemaSystem.Infrastructure.Authentication;
public sealed class JwtSettings
{
    public string Secret { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public int ExpiryHours { get; init; } = 24;
}
public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
public sealed class JwtTokenService(IOptions<JwtSettings> options) : IJwtTokenService
{
    private readonly JwtSettings _settings = options.Value;
    public string GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_settings.ExpiryHours),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}