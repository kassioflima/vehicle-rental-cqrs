using Microsoft.AspNetCore.Mvc;
using vehicle_rental.application.Common.Models;

namespace vehicle_rental.api.Middleware;

public class ResultHandlingMiddleware(RequestDelegate next, ILogger<ResultHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ResultHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
    }
}

public static class ResultHandlingExtensions
{
}
