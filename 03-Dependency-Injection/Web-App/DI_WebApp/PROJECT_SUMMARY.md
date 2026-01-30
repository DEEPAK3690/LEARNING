# DI_WebApp Project Summary

## 🎯 Project Overview

**DI_WebApp** is an ASP.NET Core Web API project that demonstrates Dependency Injection (DI) and Service Lifetimes in a real-world web application context. It complements the **DI_Demo** console application by showing how DI works in HTTP request/response scenarios.

## ✨ What's Included

### Projects
1. **DI_WebApp** - ASP.NET Core Web API (.NET 10.0)
2. **DI_WebApp.Tests** - xUnit test project (30 tests)

### Services (8 classes)
- `IOperationService` family (Transient, Scoped, Singleton demonstrations)
- `INotificationService` implementations (Email, SMS)
- `IRequestContext` (Scoped request tracking)
- `IAppConfiguration` (Singleton app configuration)
- `IOrderService` (Business service demonstrating mixed lifetimes)

### Controllers (2)
- `LifetimeController` - Demonstrates service lifetime behaviors
- `OrdersController` - Real-world business logic with DI

### Middleware (1)
- `RequestContextMiddleware` - Request tracking using scoped services

### Documentation (3 files)
- `README.md` - Comprehensive guide with API docs
- `CONSOLE_VS_WEB_COMPARISON.md` - Detailed comparison with console app
- `QUICK_REFERENCE.md` - Quick reference for developers

## 📊 Test Coverage

**30 Tests** covering:
- Service lifetime behaviors (Transient, Scoped, Singleton)
- Cross-request consistency
- Concurrent request handling
- Business logic validation
- Request correlation tracking

**Test Results:**
```
Test summary: 
  Total: 30
  Failed: 0
  Succeeded: 30
  Skipped: 0
  Duration: ~5s
```

## 🚀 Quick Start

```bash
# Run the web application
cd D:\DEV\Code\c#\DI_WebApp
dotnet run

# Access at: https://localhost:5001

# Run tests
cd D:\DEV\Code\c#\DI_WebApp.Tests
dotnet test
```

## 🔑 Key Features

### 1. Service Lifetime Demonstrations
- Visual demonstration of Transient, Scoped, Singleton behaviors
- Side-by-side comparison within single request
- Cross-request validation
- Parallel request testing

### 2. Real-World Business Logic
- Order processing with multiple service dependencies
- Request correlation with `X-Request-ID` headers
- Async operations with notifications
- Proper error handling

### 3. Middleware Integration
- Scoped service usage in middleware
- Request/response logging
- Correlation ID injection
- Performance tracking

### 4. Comprehensive Testing
- Integration tests with `WebApplicationFactory`
- Unit tests for individual services
- Concurrent request testing
- Response validation

## 📈 Learning Outcomes

After exploring this project, you will understand:

1. ✅ How DI works in ASP.NET Core
2. ✅ Service lifetime behaviors in web context
3. ✅ Automatic scope creation per HTTP request
4. ✅ Constructor injection in controllers
5. ✅ Middleware and DI integration
6. ✅ Testing web applications with DI
7. ✅ Scoped services for request-specific data
8. ✅ Singleton services for shared state
9. ✅ When to use each service lifetime
10. ✅ How to avoid captive dependencies

## 🎓 Comparison with Console App

| Feature | Console (DI_Demo) | Web (DI_WebApp) |
|---------|------------------|-----------------|
| **Scope Creation** | Manual | Automatic (per request) |
| **Service Resolution** | Explicit (`GetService`) | Implicit (injection) |
| **Entry Point** | `Main()` | Controllers/Middleware |
| **Scoped Use Case** | Less common | Very common |
| **Learning Focus** | DI fundamentals | Real-world patterns |
| **Code Complexity** | Higher boilerplate | Lower boilerplate |
| **Tests** | 33 unit tests | 30 integration tests |

## 🏗️ Architecture

```
HTTP Request
    ↓
RequestContextMiddleware (creates correlation ID)
    ↓
Controller (injects services)
    ↓
Business Services (IOrderService)
    ↓
Infrastructure Services (INotificationService)
    ↓
HTTP Response (includes X-Request-ID header)
```

**Service Dependency Chain:**
```
OrdersController
    ├── IOrderService (Scoped)
    │   ├── INotificationService (Transient)
    │   ├── IRequestContext (Scoped)
    │   └── IAppConfiguration (Singleton)
    └── ILogger (Scoped)
```

## 📝 API Endpoints

### Lifetime Demonstrations
- `GET /api/lifetime` - Basic lifetime demo
- `GET /api/lifetime/detailed` - Detailed analysis

### Business Operations
- `POST /api/orders` - Process order
- `GET /api/orders/context` - Get request context

## 🧪 Test Categories

### LifetimeControllerTests (12 tests)
- Transient service uniqueness
- Scoped service consistency within request
- Scoped service uniqueness across requests
- Singleton service consistency globally
- Parallel request handling

### OrdersControllerTests (9 tests)
- Order processing success
- Request correlation tracking
- Concurrent order processing
- Request context uniqueness
- Singleton consistency across requests

### ServiceTests (9 tests)
- Service lifetime validation
- Request context functionality
- App configuration defaults
- Notification service implementations

## 💡 Best Practices Demonstrated

1. **Interface-Based Design**
   ```csharp
   builder.Services.AddScoped<IOrderService, OrderService>();
   ```

2. **Constructor Injection**
   ```csharp
   public OrdersController(IOrderService orderService) { }
   ```

3. **Appropriate Lifetimes**
   - Transient: `INotificationService`
   - Scoped: `IRequestContext`, `IOrderService`
   - Singleton: `IAppConfiguration`

4. **Middleware with DI**
   ```csharp
   public async Task InvokeAsync(HttpContext context, IRequestContext requestContext)
   ```

5. **Action Injection**
   ```csharp
   public ActionResult Get([FromServices] IService service)
   ```

6. **Testing Patterns**
   ```csharp
   WebApplicationFactory<Program>
   ```

## 🔄 Switching Implementations

Easy to switch between implementations:

```csharp
// Change from Email to SMS
builder.Services.AddTransient<INotificationService, EmailNotificationService>();
// to
builder.Services.AddTransient<INotificationService, SmsNotificationService>();
```

No controller code changes required!

## 📚 Documentation Files

1. **README.md** (Comprehensive)
   - Project overview and structure
   - All API endpoints with examples
   - Service registration details
   - Running and testing instructions
   - Best practices
   - Comparison with console app

2. **CONSOLE_VS_WEB_COMPARISON.md** (Detailed)
   - Architecture comparison
   - Code examples side-by-side
   - Scope management differences
   - Testing approach differences
   - Migration guidance
   - Real-world scenarios

3. **QUICK_REFERENCE.md** (Quick Start)
   - Quick start commands
   - API endpoint summary
   - Service lifetime cheat sheet
   - Common pitfalls
   - Learning path

## 🎯 Use Cases

This project is ideal for:
- Learning ASP.NET Core DI
- Understanding service lifetimes in web context
- Reference for proper DI patterns
- Teaching material for DI concepts
- Starting point for new web APIs
- Interview preparation

## 🔗 Related Projects

- **DI_Demo** - Console application demonstrating DI fundamentals
- **DI_Demo.Tests** - 33 unit tests for console app

Together, these projects provide comprehensive coverage of DI in .NET!

## 📦 Technologies Used

- .NET 10.0
- ASP.NET Core Web API
- xUnit 3.1.4
- Microsoft.AspNetCore.Mvc.Testing 10.0.1
- Moq 4.20.72
- C# 13

## 🎉 Success Metrics

✅ **Build Status:** Success  
✅ **Tests:** 30/30 passing (100%)  
✅ **Code Quality:** Following best practices  
✅ **Documentation:** Comprehensive and clear  
✅ **Learning Value:** High - demonstrates real-world patterns  

## 🚦 Next Steps

1. **Run the application** - See DI in action
2. **Try the endpoints** - Understand lifetime behaviors
3. **Run the tests** - Learn testing patterns
4. **Read the comparison doc** - Understand Console vs Web
5. **Modify services** - Experiment with lifetimes
6. **Add features** - Extend with your own services

## 📞 Support

For questions or issues:
1. Check the README.md for detailed explanations
2. Review the QUICK_REFERENCE.md for common patterns
3. Read CONSOLE_VS_WEB_COMPARISON.md for deeper understanding
4. Run the tests to see expected behaviors

---

**Created:** January 6, 2026  
**Version:** 1.0.0  
**Status:** ✅ Production Ready  
**Test Coverage:** 100% passing
