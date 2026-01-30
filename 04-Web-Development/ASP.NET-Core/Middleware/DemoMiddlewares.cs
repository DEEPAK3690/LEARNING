using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApplication.Middleware
{
    public class FirstMiddleware
    {
        private readonly RequestDelegate _next;
        public FirstMiddleware(RequestDelegate next) => _next = next;
        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("[FirstMiddleware] Before next");
            await _next(context);
            Console.WriteLine("[FirstMiddleware] After next");
        }
    }

    public class SecondMiddleware
    {
        private readonly RequestDelegate _next;
        public SecondMiddleware(RequestDelegate next) => _next = next;
        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("[SecondMiddleware] Before next");
            await _next(context);
            Console.WriteLine("[SecondMiddleware] After next");
        }
    }

    public class ThirdMiddleware
    {
        private readonly RequestDelegate _next;
        public ThirdMiddleware(RequestDelegate next) => _next = next;
        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("[ThirdMiddleware] Before next");
            await _next(context);
            Console.WriteLine("[ThirdMiddleware] After next");
        }
    }
}
