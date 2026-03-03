using FluentValidation;
using CinemaSystem.Domain.Exceptions;
using System.Text.Json;
namespace CinemaSystem.WebAPI.Middleware;
public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validation failed: {Errors}", ex.Errors);
            await WriteProblemAsync(context, 422, "Validation Failed",
                detail: string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)),
                errors: ex.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));
        }
        catch (DomainException ex)
        {
            logger.LogWarning("Domain rule violation: {Message}", ex.Message);
            await WriteProblemAsync(context, 400, "Business Rule Violation", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
            await WriteProblemAsync(context, 500, "Internal Server Error",
                "An unexpected error occurred. Please try again later.");
        }
    }
    private static async Task WriteProblemAsync(HttpContext context, int statusCode,
        string title, string detail, object? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        var problem = new
        {
            type = $"https://httpstatuses.com/{statusCode}",
            title,
            status = statusCode,
            detail,
            errors,
            traceId = context.TraceIdentifier
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}