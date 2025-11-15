using OrderManagement.Contracts.Common.Errors;
using OrderManagement.Domain.Common.Exceptions;
using System.Text.Json;

namespace OrderManagement.API.Middlewares;

/// <summary>
/// Generic Global Exception Handling Middleware
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next = next;

    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    /// <summary>
    /// Invokes the asynchronous.
    /// </summary>
    /// <param name="context">The context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseException ex)
        {
            _logger.LogError(ex, nameof(BaseException));

            await HandleBaseException(context, ex);
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogError(ex, nameof(BadHttpRequestException));

            await HandleBadHttpRequestException(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await HandleUnknownException(context);
        }
    }

    private static Task HandleBaseException(HttpContext context, BaseException ex)
        => WriteJsonAsync(context, (int)ex.StatusCode, new ErrorResponse
        {
            Code = ex.ErrorCode,
            Message = ex.Message,
            StatusCode = (int)ex.StatusCode
        });

    private static Task HandleUnknownException(HttpContext context)
        => WriteJsonAsync(context, StatusCodes.Status500InternalServerError, new ErrorResponse
        {
            Code = "unknown_error",
            Message = "An unexpected error occurred.",
            StatusCode = StatusCodes.Status500InternalServerError
        });

    private static Task HandleBadHttpRequestException(HttpContext context, BadHttpRequestException ex)
        => WriteJsonAsync(context, StatusCodes.Status500InternalServerError, new ErrorResponse
        {
            Code = "bad_request",
            Message = ex.Message,
            StatusCode = ex.StatusCode
        });

    private static async Task WriteJsonAsync(HttpContext context, int status, object payload)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, Json));
    }
}
