using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace vehicle_rental.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestAuthController : ControllerBase
{
    /// <summary>
    /// Test endpoint without authentication
    /// </summary>
    [HttpGet("public")]
    public IActionResult GetPublic()
    {
        return Ok(new { message = "Public endpoint working", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Test endpoint with authentication
    /// </summary>
    [HttpGet("auth")]
    [Authorize]
    public IActionResult GetWithAuth()
    {
        var user = User.Identity?.Name;
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        
        return Ok(new { 
            message = "Authenticated endpoint working", 
            user = user,
            claims = claims,
            timestamp = DateTime.UtcNow 
        });
    }

    /// <summary>
    /// Test endpoint with admin role
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Roles = "Administrator")]
    public IActionResult GetAdmin()
    {
        var user = User.Identity?.Name;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        
        return Ok(new { 
            message = "Admin endpoint working", 
            user = user,
            roles = roles,
            timestamp = DateTime.UtcNow 
        });
    }
}
