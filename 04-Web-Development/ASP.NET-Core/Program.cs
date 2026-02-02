//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder; // Add this using directive
using Microsoft.Extensions.DependencyInjection; // Add this using directive
using Microsoft.Extensions.Hosting; // Add this using directive

using MyWebApplication.Models;

namespace MyWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Register the EmployeeRepository for Dependency Injection
            builder.Services.AddSingleton<IEmployeeRepository, EmployeeRepository>();

            builder.Services.AddControllers();//Register all controller classes with the DI container.

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //Middleware : :  When a request comes into your application, it passes through this pipeline of middleware components before generating a response.
            //is a check point for the request and response 
            // Request → Middleware 1 → Middleware 2 → Middleware 3 → Controller / Endpoint → Middleware 3 → Middleware 2 → Middleware 1 → Response

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (NotImplementedException ex)
                {
                    context.Response.StatusCode = 501; // Not Implemented
                    await context.Response.WriteAsJsonAsync(new { error = "This feature is not yet implemented", details = ex.Message });
                }
                catch (ArgumentException ex)
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid argument", details = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    context.Response.StatusCode = 409; // Conflict
                    await context.Response.WriteAsJsonAsync(new { error = "Invalid operation", details = ex.Message });
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500; // Internal Server Error
                    await context.Response.WriteAsJsonAsync(new { error = "Something went wrong", details = ex.Message });
                }
            });
            // Add demo middlewares to show the flow
            app.UseMiddleware<WebApplication.Middleware.FirstMiddleware>();
            app.UseMiddleware<WebApplication.Middleware.SecondMiddleware>();
            app.UseMiddleware<WebApplication.Middleware.ThirdMiddleware>();
            app.UseMiddleware<WebApplication.Middleware.RequestLoggingMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            


            app.Run();
        }
    }
}
