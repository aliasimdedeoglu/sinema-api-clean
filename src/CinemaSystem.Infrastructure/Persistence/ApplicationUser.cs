using Microsoft.AspNetCore.Identity;
namespace CinemaSystem.Infrastructure.Persistence;
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
}
public static class Roles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";
}