using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WebApplication.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            // Log request details before next middleware
            Console.WriteLine($"[Request] {context.Request.Method} {context.Request.Path}");

            await _next(context);

            stopwatch.Stop();
            // Log response details after next middleware
            Console.WriteLine($"[Response] {context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
