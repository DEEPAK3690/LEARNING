using DI_WebApp.Services;
using DI_WebApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ===== SERVICE LIFETIME DEMONSTRATIONS =====
// Transient: New instance every time it's requested
builder.Services.AddTransient<ITransientOperation, TransientOperation>();

// Scoped: One instance per HTTP request (per scope)
builder.Services.AddScoped<IScopedOperation, ScopedOperation>();

// Singleton: Single instance for application lifetime
builder.Services.AddSingleton<ISingletonOperation, SingletonOperation>();

// ===== BUSINESS SERVICES =====
// Notification service - Transient (new instance for each use)
builder.Services.AddTransient<INotificationService, EmailNotificationService>();
// Can switch to SMS by changing to: builder.Services.AddTransient<INotificationService, SmsNotificationService>();

// Request context - Scoped (one per request, tracks request-specific data)
builder.Services.AddScoped<IRequestContext, RequestContext>();

// Application configuration - Singleton (shared across all requests)
builder.Services.AddSingleton<IAppConfiguration, AppConfiguration>();

// Order service - Scoped (typically business services are scoped)
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Add custom middleware for request tracking (uses scoped RequestContext)
app.UseRequestContext();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Log application startup with singleton service info
var appConfig = app.Services.GetRequiredService<IAppConfiguration>();
app.Logger.LogInformation(
    "Application started: {AppName} v{Version}, Instance: {InstanceId}, Created: {CreatedAt}",
    appConfig.ApplicationName,
    appConfig.Version,
    appConfig.InstanceId,
    appConfig.CreatedAt);

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }

