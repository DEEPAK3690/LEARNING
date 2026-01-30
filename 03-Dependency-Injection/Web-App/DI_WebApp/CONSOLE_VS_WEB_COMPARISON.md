# Dependency Injection: Console vs Web Application Comparison

## Overview

This document compares how Dependency Injection works in Console applications vs Web API applications using the DI_Demo (console) and DI_WebApp (web) projects.

## Architecture Comparison

### Console Application (DI_Demo)

```
Program.cs
    ↓
Manual Service Registration
    ↓
ServiceCollection → ServiceProvider
    ↓
Manual Scope Creation
    ↓
Resolve Services Explicitly
```

### Web Application (DI_WebApp)

```
Program.cs
    ↓
WebApplicationBuilder
    ↓
ServiceCollection (builder.Services)
    ↓
Framework Creates Scopes Automatically
    ↓
Services Injected into Controllers/Middleware
```

## Service Registration

### Console

```csharp
// Manual setup
var services = new ServiceCollection();

// Register services
services.AddTransient<INotificationService, EmailNotificationService>();
services.AddScoped<IRequestContext, RequestContext>();
services.AddSingleton<IAppConfiguration, AppConfiguration>();

// Build provider
var serviceProvider = services.BuildServiceProvider();
```

### Web

```csharp
// Builder provides ServiceCollection
var builder = WebApplication.CreateBuilder(args);

// Register services - same API
builder.Services.AddTransient<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<IRequestContext, RequestContext>();
builder.Services.AddSingleton<IAppConfiguration, AppConfiguration>();

// Framework builds provider automatically
var app = builder.Build();
```

## Service Resolution

### Console - Explicit Resolution

```csharp
// Get service from provider
var service = serviceProvider.GetRequiredService<INotificationService>();

// Manual scope creation
using (var scope = serviceProvider.CreateScope())
{
    var scopedService = scope.ServiceProvider.GetRequiredService<IRequestContext>();
}
```

### Web - Automatic Injection

```csharp
// Constructor injection in controllers
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IRequestContext _requestContext;
    
    public OrdersController(
        IOrderService orderService,
        IRequestContext requestContext)
    {
        _orderService = orderService;
        _requestContext = requestContext;
    }
}
```

## Scope Management

### Console - Manual Scopes

```csharp
// Simulate 3 HTTP requests
for (int i = 0; i < 3; i++)
{
    using var scope = serviceProvider.CreateScope();
    var scopedService = scope.ServiceProvider.GetRequiredService<IScopedOperation>();
    Console.WriteLine($"Request {i}: {scopedService.OperationId}");
}
```

**Output:**
```
Request 0: guid-1  ← Different scope
Request 1: guid-2  ← Different scope
Request 2: guid-3  ← Different scope
```

### Web - Automatic Scopes

```csharp
// Framework creates scope per HTTP request
[HttpGet]
public ActionResult<Info> GetInfo(
    [FromServices] IScopedOperation scopedService)
{
    // Same scopedService instance throughout this request
    return Ok(new Info { Id = scopedService.OperationId });
}
```

**Behavior:**
- Request 1: guid-1 (framework creates scope)
- Request 2: guid-2 (framework creates new scope)
- Request 3: guid-3 (framework creates new scope)

## Service Lifetime Demonstrations

### Transient - Both Same

| Aspect | Console | Web |
|--------|---------|-----|
| **Creation** | Every `GetService()` call | Every injection point |
| **Sharing** | Never shared | Never shared within/across requests |
| **Use Case** | Stateless operations | Validators, notifications |

```csharp
// Both use same registration
services.AddTransient<ITransientOperation, TransientOperation>();
```

### Scoped - Different Behavior

| Aspect | Console | Web |
|--------|---------|-----|
| **Creation** | Per manual scope | Per HTTP request (automatic) |
| **Sharing** | Within `using` block | Within entire request pipeline |
| **Lifetime** | Manual disposal | Automatic disposal at request end |
| **Use Case** | Less common | Very common (DbContext, request data) |

**Console:**
```csharp
using (var scope = serviceProvider.CreateScope())
{
    var scoped1 = scope.ServiceProvider.GetRequiredService<IScopedOperation>();
    var scoped2 = scope.ServiceProvider.GetRequiredService<IScopedOperation>();
    // scoped1 == scoped2 (same instance)
} // Disposed here
```

**Web:**
```csharp
// Automatic scope created when request arrives
public class MyController : ControllerBase
{
    public MyController(
        IScopedOperation scoped1,    // Same instance
        IScopedOperation scoped2)    // Same instance
    {
        // scoped1 == scoped2
    }
} // Scope disposed when response is sent
```

### Singleton - Both Same

| Aspect | Console | Web |
|--------|---------|-----|
| **Creation** | First request | Application startup |
| **Sharing** | Entire app | Entire app |
| **Lifetime** | Until app exit | Until app exit |
| **Use Case** | Configuration, caches | Configuration, caches |

```csharp
// Both use same registration
services.AddSingleton<ISingletonOperation, SingletonOperation>();
```

## Testing Comparison

### Console - Direct ServiceProvider

```csharp
[Fact]
public void Test_ScopedService()
{
    // Arrange - Manual setup
    var services = new ServiceCollection();
    services.AddScoped<IScopedOperation, ScopedOperation>();
    var provider = services.BuildServiceProvider();
    
    // Act - Manual scope
    using var scope = provider.CreateScope();
    var service = scope.ServiceProvider.GetRequiredService<IScopedOperation>();
    
    // Assert
    Assert.NotEqual(Guid.Empty, service.OperationId);
}
```

### Web - WebApplicationFactory

```csharp
[Fact]
public async Task Test_ScopedService()
{
    // Arrange - Factory creates test server
    var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();
    
    // Act - HTTP request creates scope automatically
    var response = await client.GetAsync("/api/lifetime");
    var result = await response.Content.ReadFromJsonAsync<LifetimeInfo>();
    
    // Assert - Scoped services are same within request
    Assert.Equal(result.Scoped1, result.Scoped2);
}
```

## Real-World Scenarios

### Scenario 1: Database Context

**Console:**
```csharp
// Manual scope for database operations
using (var scope = serviceProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    // Use dbContext
    dbContext.SaveChanges();
} // DbContext disposed
```

**Web:**
```csharp
// Automatic scope per request
[HttpPost]
public async Task<ActionResult> CreateOrder(
    [FromServices] MyDbContext dbContext)
{
    // DbContext scoped to this request
    dbContext.Orders.Add(new Order());
    await dbContext.SaveChangesAsync();
    return Ok();
} // DbContext automatically disposed after response
```

### Scenario 2: Request Tracking

**Console:**
```csharp
// Simulate request tracking
using (var scope = serviceProvider.CreateScope())
{
    var requestContext = scope.ServiceProvider.GetRequiredService<IRequestContext>();
    requestContext.RequestId = Guid.NewGuid();
    
    // Use in services within this scope
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
    orderService.ProcessOrder(); // Can access same requestContext
}
```

**Web:**
```csharp
// Automatic request tracking via middleware
public class RequestContextMiddleware
{
    public async Task InvokeAsync(HttpContext context, IRequestContext requestContext)
    {
        // Populate request context (scoped to this request)
        requestContext.RequestId = Guid.NewGuid();
        await _next(context);
    }
}

// Controllers automatically get same instance
public class OrdersController
{
    private readonly IRequestContext _requestContext;
    
    public OrdersController(IRequestContext requestContext)
    {
        _requestContext = requestContext; // Same as middleware
    }
}
```

### Scenario 3: Notification Services

**Console:**
```csharp
var notificationService = serviceProvider.GetRequiredService<INotificationService>();
await notificationService.SendAsync("user@example.com", "Message");
```

**Web:**
```csharp
[HttpPost]
public async Task<ActionResult> SendNotification(
    [FromBody] NotificationRequest request,
    [FromServices] INotificationService notificationService)
{
    await notificationService.SendAsync(request.Email, request.Message);
    return Ok();
}
```

## Advantages & Trade-offs

### Console Application

**Advantages:**
- ✅ Explicit control over service resolution
- ✅ Clear visibility of service lifetime management
- ✅ Good for learning DI fundamentals
- ✅ Simpler debugging of scope issues

**Trade-offs:**
- ⚠️ Manual scope management required
- ⚠️ More boilerplate code
- ⚠️ Developer responsible for disposal
- ⚠️ Scoped services less natural

### Web Application

**Advantages:**
- ✅ Automatic scope per HTTP request
- ✅ Framework handles disposal
- ✅ Natural fit for scoped services
- ✅ Less boilerplate code
- ✅ Built-in middleware support

**Trade-offs:**
- ⚠️ Implicit behavior (can be confusing initially)
- ⚠️ Requires understanding of request pipeline
- ⚠️ Less control over scope boundaries

## When to Use Each

### Use Console DI When:
- Background services / Workers
- Batch processing jobs
- CLI tools
- Learning DI fundamentals
- Testing DI concepts

### Use Web DI When:
- REST APIs
- Web applications
- Real-time services (SignalR)
- Microservices
- Request/response scenarios

## Best Practices Summary

### Both Platforms

1. **Interface-based design**
   ```csharp
   services.AddTransient<INotificationService, EmailNotificationService>();
   ```

2. **Constructor injection**
   ```csharp
   public MyService(IDependency dependency) { }
   ```

3. **Appropriate lifetimes**
   - Transient: Stateless, lightweight
   - Scoped: Request-bound, DbContext
   - Singleton: Shared state, configuration

### Console-Specific

1. **Explicit scope disposal**
   ```csharp
   using var scope = provider.CreateScope();
   ```

2. **Root provider disposal**
   ```csharp
   await using var provider = services.BuildServiceProvider();
   ```

### Web-Specific

1. **Trust framework scoping**
   - Don't create manual scopes in controllers
   
2. **Use middleware for cross-cutting concerns**
   ```csharp
   app.UseRequestContext();
   ```

3. **Leverage action injection when needed**
   ```csharp
   public ActionResult Get([FromServices] IService service)
   ```

## Code Metrics Comparison

| Metric | Console (DI_Demo) | Web (DI_WebApp) |
|--------|------------------|-----------------|
| **Service Classes** | 15+ | 8 |
| **Manual Scopes** | 5+ | 0 (automatic) |
| **Controllers** | 0 | 2 |
| **Middleware** | 0 | 1 |
| **Tests** | 33 | 30 |
| **Lines of Code** | ~800 | ~600 |
| **Boilerplate** | Higher | Lower |

## Migration Path

### From Console to Web

1. **Keep service interfaces and implementations**
   - Same interfaces work in both
   
2. **Change registration location**
   ```csharp
   // From: var services = new ServiceCollection();
   // To:   var builder = WebApplication.CreateBuilder(args);
   ```

3. **Remove manual scopes**
   ```csharp
   // Remove: using var scope = provider.CreateScope();
   ```

4. **Add controllers**
   ```csharp
   public class MyController : ControllerBase
   {
       public MyController(IMyService service) { }
   }
   ```

### From Web to Console

1. **Keep service interfaces and implementations**

2. **Add manual scope management**
   ```csharp
   using var scope = serviceProvider.CreateScope();
   ```

3. **Replace controllers with direct calls**
   ```csharp
   var service = scope.ServiceProvider.GetRequiredService<IMyService>();
   service.DoWork();
   ```

## Conclusion

Both console and web applications use the same DI fundamentals, but:

- **Console apps** require explicit scope management and service resolution
- **Web apps** automate scopes (per HTTP request) and use constructor injection

Choose based on your application type:
- **Console:** Background jobs, CLI tools, learning
- **Web:** APIs, web applications, microservices

The beauty of .NET DI is that your service implementations can often move between both with minimal changes!

## Related Projects

- **DI_Demo** - Console application with detailed DI demonstrations
- **DI_WebApp** - Web API demonstrating DI in HTTP context
- **DI_Demo.Tests** - 33 unit tests for console app
- **DI_WebApp.Tests** - 30 integration tests for web app
