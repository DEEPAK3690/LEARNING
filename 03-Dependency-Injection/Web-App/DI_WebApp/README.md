# DI Web App - Dependency Injection in ASP.NET Core

## Overview

This ASP.NET Core Web API project demonstrates **Dependency Injection (DI)** and **Service Lifetimes** in a real-world web application context. It shows how DI works in HTTP request/response scenarios and how different service lifetimes behave.

## Key Concepts Demonstrated

### 1. **Service Lifetimes in Web Applications**

#### Transient
- **Created:** Every time they're requested
- **Lifespan:** No caching - new instance each time
- **Use Case:** Lightweight, stateless services (validators, calculators)
- **Example:** `INotificationService` - each operation gets fresh instance

#### Scoped
- **Created:** Once per HTTP request
- **Lifespan:** Lives for the duration of the request
- **Use Case:** Request-specific data, database contexts
- **Example:** `IRequestContext` - tracks request ID and data per request

#### Singleton
- **Created:** Once when first requested
- **Lifespan:** Entire application lifetime
- **Use Case:** Shared state, configuration, caches
- **Example:** `IAppConfiguration` - application settings shared across all requests

### 2. **Dependency Injection Benefits**

- **Loose Coupling:** Controllers depend on interfaces, not implementations
- **Testability:** Easy to mock dependencies in unit tests
- **Flexibility:** Can swap implementations (e.g., Email → SMS notifications) via configuration
- **Maintainability:** Changes isolated to service implementations

## Project Structure

```
DI_WebApp/
├── Controllers/
│   ├── LifetimeController.cs      # Demonstrates service lifetime behaviors
│   └── OrdersController.cs        # Real-world business logic with DI
├── Services/
│   ├── IOperationService.cs       # Interfaces for lifetime demos
│   ├── OperationService.cs        # Implementations with unique IDs
│   ├── INotificationService.cs    # Abstraction for notifications
│   ├── EmailNotificationService.cs
│   ├── SmsNotificationService.cs
│   ├── IRequestContext.cs         # Request-scoped tracking
│   ├── IAppConfiguration.cs       # Singleton configuration
│   └── OrderService.cs            # Business service using multiple dependencies
├── Middleware/
│   └── RequestContextMiddleware.cs # Tracks requests using scoped services
└── Program.cs                     # Service registration and configuration

DI_WebApp.Tests/
├── LifetimeControllerTests.cs     # Integration tests for lifetime behaviors
├── OrdersControllerTests.cs       # Tests for business logic
└── ServiceTests.cs                # Unit tests for services
```

## API Endpoints

### Lifetime Demonstration

#### `GET /api/lifetime`
Demonstrates how different service lifetimes behave within a single request.

**Response:**
```json
{
  "controllerId": "guid",
  "transient1": "guid",      // Different from transient2
  "transient2": "guid",      // Different from transient1
  "scoped1": "guid",         // Same as scoped2
  "scoped2": "guid",         // Same as scoped1
  "singleton1": "guid",      // Same as singleton2 and across all requests
  "singleton2": "guid"       // Same as singleton1 and across all requests
}
```

**Key Observations:**
- Transient1 ≠ Transient2 (new instance each time)
- Scoped1 = Scoped2 (same within request)
- Singleton1 = Singleton2 = Same across ALL requests

#### `GET /api/lifetime/detailed`
Provides detailed analysis of service lifetime behaviors including action-injected services.

### Business Logic Demonstration

#### `POST /api/orders`
Processes an order using multiple services with different lifetimes.

**Request:**
```json
{
  "orderId": "ORDER-001",
  "customerId": "CUST-123"
}
```

**Response:**
```json
{
  "success": true,
  "orderId": "ORDER-001",
  "customerId": "CUST-123",
  "processedAt": "2025-01-06T10:30:00Z",
  "requestId": "guid",       // From scoped RequestContext
  "message": "Order processed successfully"
}
```

**Response Headers:**
- `X-Request-ID`: Correlation ID from RequestContext (scoped)

#### `GET /api/orders/context`
Shows request context and application configuration information.

**Response:**
```json
{
  "requestId": "guid",              // Changes per request (Scoped)
  "requestTime": "2025-01-06T10:30:00Z",
  "userId": "Anonymous",
  "applicationName": "DI Web App Demo",
  "applicationVersion": "1.0.0",
  "appInstanceId": "guid",          // Same across all requests (Singleton)
  "appCreatedAt": "2025-01-06T10:00:00Z"
}
```

## Service Registration (Program.cs)

```csharp
// Transient - New every time
builder.Services.AddTransient<ITransientOperation, TransientOperation>();
builder.Services.AddTransient<INotificationService, EmailNotificationService>();

// Scoped - One per request
builder.Services.AddScoped<IScopedOperation, ScopedOperation>();
builder.Services.AddScoped<IRequestContext, RequestContext>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Singleton - One for application lifetime
builder.Services.AddSingleton<ISingletonOperation, SingletonOperation>();
builder.Services.AddSingleton<IAppConfiguration, AppConfiguration>();
```

## Middleware

### RequestContextMiddleware
- Uses scoped `IRequestContext` service
- Tracks request ID, path, method, timing
- Adds `X-Request-ID` header to responses
- Demonstrates how middleware can use DI

## Running the Application

### Prerequisites
- .NET 10.0 or later
- Visual Studio 2022, VS Code, or Rider

### Run the Web App
```bash
cd DI_WebApp
dotnet run
```

The app will start on `https://localhost:5001` (or similar).

### Test the Endpoints

**Test Lifetime Behavior:**
```bash
# Make multiple requests to see lifetime differences
curl https://localhost:5001/api/lifetime
curl https://localhost:5001/api/lifetime
curl https://localhost:5001/api/lifetime
```

**Process Orders:**
```bash
curl -X POST https://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{"orderId":"ORDER-001","customerId":"CUST-123"}'
```

**Check Request Context:**
```bash
curl https://localhost:5001/api/orders/context
```

### Run Tests
```bash
cd DI_WebApp.Tests
dotnet test
```

**Test Coverage:**
- 29 unit and integration tests
- Tests for all three service lifetimes
- Tests for concurrent requests
- Tests for middleware behavior
- Tests for business logic

## Key Learnings

### 1. Service Lifetime Behaviors

| Lifetime   | Within Request | Across Requests | Across Scopes |
|-----------|---------------|-----------------|---------------|
| Transient | Different     | Different       | Different     |
| Scoped    | Same          | Different       | Different     |
| Singleton | Same          | Same            | Same          |

### 2. When to Use Each Lifetime

**Use Transient for:**
- Lightweight services with no state
- Services that shouldn't be shared
- Validators, calculators, formatters

**Use Scoped for:**
- Database contexts (Entity Framework Core)
- Request-specific data (user context, correlation IDs)
- Business services that work within a request
- Services that depend on scoped services

**Use Singleton for:**
- Application configuration
- Caches and shared state
- Expensive-to-create objects that are thread-safe
- Services that should be shared across the app

### 3. Dependency Chain Rules

⚠️ **Important:** A service can only depend on services with equal or longer lifetimes:

✅ **Valid:**
- Transient → Scoped ❌ (Transient is shorter)
- Transient → Singleton ✅
- Scoped → Singleton ✅
- Singleton → Singleton ✅

❌ **Invalid (Captive Dependency):**
- Singleton → Scoped ❌ (Scoped would be "captured")
- Singleton → Transient ❌
- Scoped → Transient ❌ (Generally avoid, but less problematic)

## Comparing Console vs Web DI

| Aspect | Console (DI_Demo) | Web (DI_WebApp) |
|--------|------------------|-----------------|
| **Scopes** | Manual with `CreateScope()` | Automatic per HTTP request |
| **Entry Point** | `Main()` method | `Program.cs` with WebApplication builder |
| **Service Access** | Direct from `ServiceProvider` | Injected into controllers/middleware |
| **Lifetime Demo** | Simulated with manual scopes | Real HTTP request boundaries |
| **Scoped Use Case** | Less common | Very common (per-request data) |

## Testing

### Integration Tests with WebApplicationFactory

```csharp
public class LifetimeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public LifetimeControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetLifetimeInfo_SingletonServices_HaveSameIdsAcrossRequests()
    {
        var client = _factory.CreateClient();
        
        var response1 = await client.GetAsync("/api/lifetime");
        var result1 = await response1.Content.ReadFromJsonAsync<LifetimeInfo>();
        
        var response2 = await client.GetAsync("/api/lifetime");
        var result2 = await response2.Content.ReadFromJsonAsync<LifetimeInfo>();
        
        // Singleton should be the same
        Assert.Equal(result1.Singleton1, result2.Singleton1);
    }
}
```

### Unit Tests with Manual DI Container

```csharp
[Fact]
public void TransientOperation_CreatesNewInstanceEachTime()
{
    var services = new ServiceCollection();
    services.AddTransient<ITransientOperation, TransientOperation>();
    var provider = services.BuildServiceProvider();
    
    var instance1 = provider.GetRequiredService<ITransientOperation>();
    var instance2 = provider.GetRequiredService<ITransientOperation>();
    
    Assert.NotEqual(instance1.OperationId, instance2.OperationId);
}
```

## Best Practices Demonstrated

1. ✅ **Interface-based design** - All services use interfaces
2. ✅ **Constructor injection** - Dependencies injected via constructor
3. ✅ **Appropriate lifetimes** - Each service has correct lifetime
4. ✅ **Logging integration** - Services log important events
5. ✅ **Correlation IDs** - Track requests across services
6. ✅ **Comprehensive testing** - Both unit and integration tests
7. ✅ **Clear separation** - Controllers, Services, Middleware

## Comparison with Console DI_Demo

Both projects demonstrate the same DI concepts but in different contexts:

- **Console App:** Manual scope management, explicit service resolution
- **Web App:** Automatic scope per request, framework-managed injection

This web app shows how ASP.NET Core makes DI even more powerful with:
- Automatic scope creation per HTTP request
- Middleware integration with DI
- Easy testing with WebApplicationFactory
- Real-world HTTP request/response scenarios

## Next Steps

1. **Experiment:** Change service lifetimes in `Program.cs` and observe behavior
2. **Swap Implementations:** Change from `EmailNotificationService` to `SmsNotificationService`
3. **Add Services:** Create new services with different lifetimes
4. **Add Tests:** Write more tests to verify behaviors
5. **Try Captive Dependencies:** See what happens when you violate lifetime rules

## References

- [ASP.NET Core Dependency Injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Service Lifetimes](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)
- [Testing in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
