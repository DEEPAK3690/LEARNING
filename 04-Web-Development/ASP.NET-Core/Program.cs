//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using MyWebApplication.Models;
using MyWebApplication.Models.DIExamples;
using MyWebApplication.Services;

namespace MyWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

            // ============ CONFIGURATION ============
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"] ?? "this-is-a-super-secret-key-min-32-characters-long!!!";
            var issuer = jwtSettings["Issuer"] ?? "MyWebApplication";
            var audience = jwtSettings["Audience"] ?? "MyWebApplicationUsers";

            // Add services to the container.
            // Register the EmployeeRepository for Dependency Injection
            builder.Services.AddSingleton<IEmployeeRepository, EmployeeRepository>();

            // ============ DEPENDENCY INJECTION - ALL LIFETIMES ============
            // TRANSIENT: New instance created every time it's requested
            builder.Services.AddTransient<ITransientService, TransientService>();

            // SCOPED: One instance per HTTP request
            builder.Services.AddScoped<IScopedService, ScopedService>();

            // SINGLETON: One instance for entire application lifetime
            builder.Services.AddSingleton<ISingletonService, SingletonService>();
            // ===============================================================

            // ============ AUTHENTICATION & AUTHORIZATION ============
            // Register authentication services
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Configure Authorization
            builder.Services.AddAuthorization(options =>
            {
                // You can add custom authorization policies here if needed
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("ManagerOrAdmin", policy =>
                    policy.RequireRole("Manager", "Admin"));
            });
            // ===============================================================

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

            app.UseHttpsRedirection();

            // ============ AUTHENTICATION & AUTHORIZATION MIDDLEWARE ============
            // ORDER MATTERS! Must be in this order:
            // 1. UseAuthentication - Identifies the user (who are you?)
            // 2. UseAuthorization - Checks permissions (what can you do?)
            app.UseAuthentication();  // Validates JWT token
            app.UseAuthorization();   // Checks roles and permissions
            // ===============================================================

            app.MapControllers();

            app.Run();
        }
    }
}
