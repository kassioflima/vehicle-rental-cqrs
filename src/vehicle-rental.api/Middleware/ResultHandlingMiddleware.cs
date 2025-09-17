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
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result is not null && result.IsSuccess)
        {
            // Sempre retorna 200 OK quando o resultado é bem-sucedido
            return new OkObjectResult(result.Value);
        }

        return result?.Error?.ToErrorResponse<T>() ??
            new BadRequestObjectResult(new 
            { 
                code = "UNKNOWN_ERROR", 
                message = "An unknown error occurred" 
            });
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkResult();
        }

        return result.Error?.ToErrorResponse() ?? new BadRequestObjectResult(new { code = "UNKNOWN_ERROR", message = "An unknown error occurred" });
    }

    public static IActionResult ToActionResultWithoutValue<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkResult();
        }

        return result.Error?.ToErrorResponse() ?? new BadRequestObjectResult(new { code = "UNKNOWN_ERROR", message = "An unknown error occurred" });
    }

    private static ActionResult<T> ToErrorResponse<T>(this Error error) => error.Code switch
    {
        "NOT_FOUND" => new NotFoundObjectResult(new { code = error.Code, message = error.Message }),
        "ALREADY_EXISTS" => new ConflictObjectResult(new { code = error.Code, message = error.Message }),
        "INVALID_OPERATION" => new BadRequestObjectResult(new { code = error.Code, message = error.Message }),
        "VALIDATION_FAILED" => new BadRequestObjectResult(new { code = error.Code, message = error.Message }),
        "UNAUTHORIZED" => new UnauthorizedObjectResult(new { code = error.Code, message = error.Message }),
        "FORBIDDEN" => new ObjectResult(new { code = error.Code, message = error.Message }) { StatusCode = 403 },
        "CONFLICT" => new ConflictObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_INVALID_CREDENTIALS" => new UnauthorizedObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_USER_NOT_FOUND" => new UnauthorizedObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_USER_ALREADY_EXISTS" => new ConflictObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_INSUFFICIENT_PERMISSIONS" => new ObjectResult(new { code = error.Code, message = error.Message }) { StatusCode = 403 },
        "AUTH_VALIDATION_ERROR" => new BadRequestObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_USER_BLOCKED" => new ObjectResult(new { code = error.Code, message = error.Message }) { StatusCode = 403 },
        "AUTH_USER_LOCKED_OUT" => new ObjectResult(new { code = error.Code, message = error.Message }) { StatusCode = 423 },
        _ => new BadRequestObjectResult(new { code = error.Code, message = error.Message })
    };

    private static IActionResult ToErrorResponse(this Error error) => error.Code switch
    {
        "NOT_FOUND" => new NotFoundObjectResult(new { code = error.Code, message = error.Message }),
        "ALREADY_EXISTS" => new ConflictObjectResult(new { code = error.Code, message = error.Message }),
        "INVALID_OPERATION" => new BadRequestObjectResult(new { code = error.Code, message = error.Message }),
        "VALIDATION_FAILED" => new BadRequestObjectResult(new { code = error.Code, message = error.Message }),
        "UNAUTHORIZED" => new UnauthorizedObjectResult(new { code = error.Code, message = error.Message }),
        "FORBIDDEN" => new ObjectResult(new { code = error.Code, message = error.Message }) { StatusCode = 403 },
        "CONFLICT" => new ConflictObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_INVALID_CREDENTIALS" => new UnauthorizedObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_USER_NOT_FOUND" => new UnauthorizedObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_USER_ALREADY_EXISTS" => new ConflictObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_INSUFFICIENT_PERMISSIONS" => new ObjectResult(new { code = error.Code, message = error.Message }) { StatusCode = 403 },
        "AUTH_VALIDATION_ERROR" => new BadRequestObjectResult(new { code = error.Code, message = error.Message }),
        "AUTH_USER_BLOCKED" => new ObjectResult(new { code = error.Code, message = error.Message }) { StatusCode = 403 },
        "AUTH_USER_LOCKED_OUT" => new ObjectResult(new { code = error.Code, message = error.Message }) { StatusCode = 423 },
        _ => new BadRequestObjectResult(new { code = error.Code, message = error.Message })
    };
}
