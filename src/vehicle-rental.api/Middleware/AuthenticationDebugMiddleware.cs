using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace vehicle_rental.api.Middleware;

public class AuthenticationDebugMiddleware(RequestDelegate next, ILogger<AuthenticationDebugMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<AuthenticationDebugMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        // Log request details
        _logger.LogInformation("=== AUTH DEBUG ===");
        _logger.LogInformation("Path: {Path}", context.Request.Path);
        _logger.LogInformation("Method: {Method}", context.Request.Method);
        
        // Check for Authorization header
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            _logger.LogInformation("Authorization Header: {AuthHeader}", authHeader.ToString());
            
            if (authHeader.ToString().StartsWith("Bearer "))
            {
                var token = authHeader.ToString().Substring(7);
                _logger.LogInformation("Token: {Token}", token);
                
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadJwtToken(token);
                    
                    _logger.LogInformation("Token Claims:");
                    foreach (var claim in jsonToken.Claims)
                    {
                        _logger.LogInformation("  {Type}: {Value}", claim.Type, claim.Value);
                    }
                    
                    _logger.LogInformation("Token Expiry: {Expiry}", jsonToken.ValidTo);
                    _logger.LogInformation("Current Time: {CurrentTime}", DateTime.UtcNow);
                    _logger.LogInformation("Is Expired: {IsExpired}", jsonToken.ValidTo < DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing JWT token");
                }
            }
        }
        else
        {
            _logger.LogInformation("No Authorization header found");
        }
        
        _logger.LogInformation("==================");

        await _next(context);
        
        // Log response details
        _logger.LogInformation("Response Status: {StatusCode}", context.Response.StatusCode);
        _logger.LogInformation("==================");
    }
}
