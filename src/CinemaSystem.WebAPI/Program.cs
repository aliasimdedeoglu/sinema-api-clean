using CinemaSystem.Application;
using CinemaSystem.Infrastructure;
using CinemaSystem.Infrastructure.Persistence;
using CinemaSystem.WebAPI.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/cinema-.txt", rollingInterval: RollingInterval.Day));
    builder.Services.AddApplication();
    if (builder.Environment.IsEnvironment("Testing"))
    {
        builder.Services.AddInfrastructure(
            builder.Configuration,
            opts => opts.UseInMemoryDatabase("CinemaSystemIntegrationTest"));
    }
    else
    {
        builder.Services.AddInfrastructure(builder.Configuration);
    }
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(o =>
            o.SuppressModelStateInvalidFilter = true);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Cinema System API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
    var app = builder.Build();
    if (!app.Environment.IsEnvironment("Testing"))
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
        db.Database.Migrate();
    }
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseSerilogRequestLogging();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }