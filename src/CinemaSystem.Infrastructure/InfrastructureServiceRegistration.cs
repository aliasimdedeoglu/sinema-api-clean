using CinemaSystem.Application.Features.Reservations.Commands;
using CinemaSystem.Domain.Repositories;
using CinemaSystem.Infrastructure.Authentication;
using CinemaSystem.Infrastructure.Persistence;
using CinemaSystem.Infrastructure.Repositories;
using CinemaSystem.Infrastructure.Transactions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace CinemaSystem.Infrastructure;
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
        => services.AddInfrastructure(configuration, dbOptions =>
        {
            dbOptions.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(3); // Handle transient SQL errors
                    sqlOptions.CommandTimeout(30);
                });
        });
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<CinemaDbContext>(configureDb);
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false; // Enable in production
            })
            .AddEntityFrameworkStores<CinemaDbContext>()
            .AddDefaultTokenProviders();
        var jwtSettings = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSettings);
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
                    ClockSkew = TimeSpan.Zero // No tolerance on expiry — enforce strictly
                };
            });
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<ICinemaHallRepository, CinemaHallRepository>();
        services.AddScoped<IShowTimeRepository, ShowTimeRepository>();
        services.AddScoped<ISeatRepository, SeatRepository>();
        services.AddScoped<IShowTimeSeatRepository, ShowTimeSeatRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IDbTransactionManager, DbTransactionManager>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        return services;
    }
}