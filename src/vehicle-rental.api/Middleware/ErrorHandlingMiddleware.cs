using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace vehicle_rental.api.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        // Log incoming request details
        _logger.LogInformation("ErrorHandlingMiddleware: {Method} {Path} - Status: {StatusCode}", 
            context.Request.Method, 
            context.Request.Path, 
            context.Response.StatusCode);

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Only handle exceptions if the response hasn't been started
            if (!context.Response.HasStarted)
            {
                _logger.LogError(ex, "An unhandled exception occurred for {Method} {Path}", 
                    context.Request.Method, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
            else
            {
                // If response has started, re-throw to let the framework handle it
                throw;
            }
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var problemDetails = exception switch
        {
            ArgumentNullException => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Detail = exception.Message,
                Instance = context.Request.Path
            },
            ArgumentException => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Detail = exception.Message,
                Instance = context.Request.Path
            },
            InvalidOperationException => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Detail = exception.Message,
                Instance = context.Request.Path
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Title = "Unauthorized",
                Detail = exception.Message,
                Instance = context.Request.Path
            },
            NotImplementedException => new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotImplemented,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.2",
                Title = "Not Implemented",
                Detail = exception.Message,
                Instance = context.Request.Path
            },
            _ => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Detail = "An error occurred while processing your request.",
                Instance = context.Request.Path
            }
        };

        context.Response.StatusCode = problemDetails.Status ?? 500;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
