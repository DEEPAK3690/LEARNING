using DI_WebApp.Services;

namespace DI_WebApp.Middleware;

/// <summary>
/// Middleware that tracks request information using scoped RequestContext.
/// Demonstrates how middleware can use DI and scoped services.
/// </summary>
public class RequestContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestContextMiddleware> _logger;

    public RequestContextMiddleware(
        RequestDelegate next,
        ILogger<RequestContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IRequestContext requestContext)
    {
        // Populate request context with information from HTTP context
        requestContext.UserId = context.User?.Identity?.Name ?? "Anonymous";
        requestContext.Data["Path"] = context.Request.Path;
        requestContext.Data["Method"] = context.Request.Method;
        requestContext.Data["QueryString"] = context.Request.QueryString.ToString();

        _logger.LogInformation(
            "Request started - ID: {RequestId}, Time: {RequestTime}, Path: {Path}, Method: {Method}",
            requestContext.RequestId,
            requestContext.RequestTime,
            context.Request.Path,
            context.Request.Method);

        // Add correlation ID to response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Append("X-Request-ID", requestContext.RequestId.ToString());
            return Task.CompletedTask;
        });

        try
        {
            await _next(context);
            
            _logger.LogInformation(
                "Request completed - ID: {RequestId}, Status: {StatusCode}, Duration: {Duration}ms",
                requestContext.RequestId,
                context.Response.StatusCode,
                (DateTime.UtcNow - requestContext.RequestTime).TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Request failed - ID: {RequestId}, Error: {ErrorMessage}",
                requestContext.RequestId,
                ex.Message);
            throw;
        }
    }
}

/// <summary>
/// Extension method to register the middleware
/// </summary>
public static class RequestContextMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestContext(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestContextMiddleware>();
    }
}
