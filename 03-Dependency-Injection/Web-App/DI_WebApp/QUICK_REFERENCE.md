# DI_WebApp Quick Reference

## 🚀 Quick Start

```bash
# Run the web application
cd D:\DEV\Code\c#\DI_WebApp
dotnet run

# Run tests
cd D:\DEV\Code\c#\DI_WebApp.Tests
dotnet test
```

## 📋 API Endpoints

### Lifetime Demonstrations

#### Basic Lifetime Demo
```bash
GET /api/lifetime
```
**Shows:** How Transient, Scoped, and Singleton services behave within a single request

#### Detailed Lifetime Demo
```bash
GET /api/lifetime/detailed
```
**Shows:** Service lifetime analysis with constructor and action injection

### Business Logic

#### Process Order
```bash
POST /api/orders
Content-Type: application/json

{
  "orderId": "ORDER-001",
  "customerId": "CUST-123"
}
```

#### Get Request Context
```bash
GET /api/orders/context
```
**Shows:** Request-scoped context and singleton app configuration

## 🎯 Service Lifetimes

### Transient ⚡
- **Lifetime:** New instance every time
- **Registration:** `AddTransient<I, T>()`
- **Example:** `INotificationService`
- **Use for:** Validators, calculators, lightweight services

### Scoped 🔄
- **Lifetime:** One instance per HTTP request
- **Registration:** `AddScoped<I, T>()`
- **Example:** `IRequestContext`, `IOrderService`
- **Use for:** DbContext, request-specific data, business services

### Singleton 🌐
- **Lifetime:** One instance for app lifetime
- **Registration:** `AddSingleton<I, T>()`
- **Example:** `IAppConfiguration`
- **Use for:** Configuration, caches, shared state

## 🔧 Service Registration (Program.cs)

```csharp
// Transient
builder.Services.AddTransient<ITransientOperation, TransientOperation>();
builder.Services.AddTransient<INotificationService, EmailNotificationService>();

// Scoped
builder.Services.AddScoped<IScopedOperation, ScopedOperation>();
builder.Services.AddScoped<IRequestContext, RequestContext>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Singleton
builder.Services.AddSingleton<ISingletonOperation, SingletonOperation>();
builder.Services.AddSingleton<IAppConfiguration, AppConfiguration>();
```

## 🧪 Testing

### Run All Tests (30 tests)
```bash
dotnet test
```

### Test Categories
- **LifetimeControllerTests:** 12 tests - Service lifetime behaviors
- **OrdersControllerTests:** 9 tests - Business logic and request tracking
- **ServiceTests:** 9 tests - Unit tests for services

### Key Tests
```csharp
// Verify transients differ
GetLifetimeInfo_TransientServices_HaveDifferentIds()

// Verify scoped same within request
GetLifetimeInfo_ScopedServices_HaveSameIdWithinRequest()

// Verify scoped differ across requests
GetLifetimeInfo_ScopedServices_HaveDifferentIdsAcrossRequests()

// Verify singleton same everywhere
GetLifetimeInfo_SingletonServices_HaveSameIdsAcrossRequests()
```

## 🏗️ Project Structure

```
DI_WebApp/
├── Controllers/
│   ├── LifetimeController.cs     # Lifetime demonstrations
│   └── OrdersController.cs       # Business logic
├── Services/
│   ├── IOperationService.cs      # Lifetime interfaces
│   ├── OperationService.cs       # Lifetime implementations
│   ├── INotificationService.cs
│   ├── EmailNotificationService.cs
│   ├── SmsNotificationService.cs
│   ├── IRequestContext.cs        # Scoped request tracking
│   ├── IAppConfiguration.cs      # Singleton config
│   └── OrderService.cs           # Business service
├── Middleware/
│   └── RequestContextMiddleware.cs
└── Program.cs                    # Service registration

DI_WebApp.Tests/
├── LifetimeControllerTests.cs
├── OrdersControllerTests.cs
└── ServiceTests.cs
```

## 📊 Expected Behaviors

### Within Single Request
```
Transient1: guid-1  ≠  Transient2: guid-2  (Different)
Scoped1:    guid-3  =  Scoped2:    guid-3  (Same)
Singleton1: guid-4  =  Singleton2: guid-4  (Same)
```

### Across Multiple Requests
```
Request 1:
  Transient: guid-1, guid-2
  Scoped:    guid-3
  Singleton: guid-4

Request 2:
  Transient: guid-5, guid-6  (New)
  Scoped:    guid-7          (New)
  Singleton: guid-4          (Same as Request 1)
```

## 🎨 Switching Implementations

### Change from Email to SMS Notifications
In [Program.cs](Program.cs):
```csharp
// From:
builder.Services.AddTransient<INotificationService, EmailNotificationService>();

// To:
builder.Services.AddTransient<INotificationService, SmsNotificationService>();
```

## 📝 Response Headers

All responses include:
- `X-Request-ID`: Correlation ID from scoped `IRequestContext`

## 🔍 Debugging Tips

### View Service Creation
Check logs for service instance creation:
```
info: DI_WebApp.Services.EmailNotificationService[0]
      EmailNotificationService created with ID: {guid}
```

### Track Request Flow
Middleware logs:
```
info: DI_WebApp.Middleware.RequestContextMiddleware[0]
      Request started - ID: {guid}, Path: /api/orders
```

### Verify Singleton Consistency
Same `AppInstanceId` across all requests:
```bash
curl http://localhost:5000/api/orders/context
# Note AppInstanceId

curl http://localhost:5000/api/orders/context
# AppInstanceId should be identical
```

## ⚠️ Common Pitfalls

### ❌ Captive Dependency
```csharp
// BAD: Singleton capturing Scoped
builder.Services.AddSingleton<IMySingleton, MySingleton>();
public class MySingleton
{
    // This scoped service gets "captured"
    public MySingleton(IScopedOperation scoped) { }
}
```

### ✅ Correct Pattern
```csharp
// GOOD: Singleton → Singleton, Scoped → Scoped
builder.Services.AddSingleton<IAppConfiguration, AppConfiguration>();
builder.Services.AddScoped<IOrderService, OrderService>();

public class OrderService  // Scoped
{
    // Scoped can depend on Singleton
    public OrderService(IAppConfiguration config) { }
}
```

## 📚 Key Learnings

1. **Transient** = New every time → Use for stateless operations
2. **Scoped** = One per request → Use for DbContext, request data
3. **Singleton** = One forever → Use for configuration, caches
4. **Web Framework** automatically creates scopes per HTTP request
5. **Middleware** can inject scoped services
6. **Controllers** use constructor injection
7. **Action parameters** can use `[FromServices]` injection

## 🔗 Related Documentation

- [README.md](README.md) - Comprehensive guide
- [CONSOLE_VS_WEB_COMPARISON.md](CONSOLE_VS_WEB_COMPARISON.md) - Console vs Web comparison
- [Microsoft Docs - DI in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)

## 🎓 Learning Path

1. ✅ Run `GET /api/lifetime` - See basic lifetime behaviors
2. ✅ Run multiple times - Observe Singleton consistency
3. ✅ Run `GET /api/lifetime/detailed` - See detailed analysis
4. ✅ Run `POST /api/orders` - See business service interaction
5. ✅ Check response headers - See `X-Request-ID`
6. ✅ Run tests - Understand testing patterns
7. ✅ Switch to SMS - See implementation swapping
8. ✅ Read comparison doc - Understand Console vs Web differences

## 💡 Next Steps

- Experiment with different service lifetimes
- Add your own services
- Try violating captive dependency rules (see errors)
- Add more controllers and middleware
- Integrate with Entity Framework Core
- Add authentication/authorization
