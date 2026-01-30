# DI Quick Reference Guide

## 🚀 Quick Start Commands

```bash
# Run the demo application
cd c#/DI_Demo
dotnet run

# Run all tests
cd c#/DI_Demo.Tests
dotnet test

# Run with detailed test output
dotnet test --verbosity detailed

# Build both projects
cd c#
dotnet build DI_Demo.sln
```

## 📝 Cheat Sheet

### 1. Define Interface (Abstraction)
```csharp
public interface INotificationService
{
    void SendNotification(string recipient, string message);
}
```

### 2. Implement Interface
```csharp
public class EmailNotificationService : INotificationService
{
    public void SendNotification(string recipient, string message)
    {
        // Implementation
    }
}
```

### 3. Use Dependency Injection (Constructor)
```csharp
public class UserService
{
    private readonly INotificationService _notificationService;
    
    public UserService(INotificationService notificationService)
    {
        _notificationService = notificationService 
            ?? throw new ArgumentNullException(nameof(notificationService));
    }
    
    public void DoWork()
    {
        _notificationService.SendNotification("user@email.com", "Hello!");
    }
}
```

### 4. Register Services (DI Container)
```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register interface with implementation
services.AddTransient<INotificationService, EmailNotificationService>();
services.AddTransient<UserService>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get service (dependencies auto-injected)
var userService = serviceProvider.GetRequiredService<UserService>();
```

## 🧪 Testing Pattern

### 1. Install Moq
```bash
dotnet add package Moq
```

### 2. Create Mock and Test
```csharp
using Moq;
using Xunit;

[Fact]
public void Test_Method_CallsDependency()
{
    // Arrange - Create mock
    var mockService = new Mock<INotificationService>();
    var userService = new UserService(mockService.Object);
    
    // Act - Execute
    userService.DoWork();
    
    // Assert - Verify
    mockService.Verify(
        x => x.SendNotification(
            It.IsAny<string>(), 
            It.IsAny<string>()
        ),
        Times.Once
    );
}
```

## 🔄 Service Lifetimes

```csharp
// Singleton - One instance for entire app
services.AddSingleton<IConfigService, ConfigService>();

// Transient - New instance every time
services.AddTransient<INotificationService, EmailService>();

// Scoped - One instance per scope (e.g., per HTTP request)
services.AddScoped<IDbContext, AppDbContext>();
```

## ✅ DI Checklist

When creating a new service:

- [ ] Define interface for dependencies
- [ ] Inject dependencies through constructor
- [ ] Validate parameters (null checks)
- [ ] Register service in DI container
- [ ] Write unit tests with mocks
- [ ] Choose appropriate lifetime

## 🎯 Common Patterns

### Pattern 1: Multiple Dependencies
```csharp
public class PaymentService
{
    private readonly INotificationService _notification;
    private readonly ILoggerService _logger;
    
    public PaymentService(
        INotificationService notification,
        ILoggerService logger)
    {
        _notification = notification;
        _logger = logger;
    }
}
```

### Pattern 2: Optional Dependencies (Property Injection)
```csharp
public class UserService
{
    // Required dependency (constructor)
    private readonly INotificationService _notification;
    
    // Optional dependency (property)
    public ILoggerService? Logger { get; set; }
    
    public UserService(INotificationService notification)
    {
        _notification = notification;
    }
}
```

### Pattern 3: Factory Pattern with DI
```csharp
public interface INotificationFactory
{
    INotificationService Create(string type);
}

public class NotificationFactory : INotificationFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public NotificationFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public INotificationService Create(string type)
    {
        return type switch
        {
            "email" => _serviceProvider.GetRequiredService<EmailNotificationService>(),
            "sms" => _serviceProvider.GetRequiredService<SmsNotificationService>(),
            _ => throw new ArgumentException("Unknown type")
        };
    }
}
```

## 🔍 Mock Verification Examples

```csharp
// Verify method called once
mock.Verify(x => x.Method(), Times.Once);

// Verify method never called
mock.Verify(x => x.Method(), Times.Never);

// Verify with specific parameters
mock.Verify(x => x.Method("test", 42), Times.Once);

// Verify with matchers
mock.Verify(
    x => x.Method(
        It.IsAny<string>(),
        It.Is<int>(n => n > 0)
    ),
    Times.Once
);

// Setup return value
mock.Setup(x => x.GetValue()).Returns(42);

// Setup to throw exception
mock.Setup(x => x.Method()).Throws<Exception>();
```

## 📚 Project Structure Reference

```
DI_Demo/                          # Main project
├── WithoutDI.cs                  # Examples WITHOUT DI
├── WithDI.cs                     # Examples WITH DI
├── Program.cs                    # Demo application
├── README.md                     # Documentation
└── DI_COMPARISON.md              # Detailed comparison

DI_Demo.Tests/                    # Test project
├── WithoutDI_Tests.cs            # Tests showing difficulty
└── WithDI_Tests.cs               # Tests showing ease

DI_Demo.sln                       # Solution file
```

## 💡 Tips & Best Practices

1. **Always use interfaces** for dependencies
2. **Constructor injection** for required dependencies
3. **Property injection** for optional dependencies
4. **Validate parameters** in constructor (null checks)
5. **Keep interfaces focused** (ISP - Interface Segregation)
6. **Register at composition root** (Program.cs, Startup.cs)
7. **Choose appropriate lifetime** (Singleton, Transient, Scoped)
8. **Write tests with mocks** to verify behavior
9. **Avoid circular dependencies**
10. **Use meaningful interface names** (INotificationService, not INotifier)

## 🐛 Troubleshooting

### "No service for type X"
```csharp
// Solution: Register the service
services.AddTransient<IMyService, MyService>();
```

### "Circular dependency detected"
```csharp
// Solution: Refactor to remove circular reference
// or use IServiceProvider to resolve lazily
```

### "Cannot resolve service"
```csharp
// Solution: Make sure all dependencies are registered
// Check constructor parameters
```

## 📖 Further Reading

- [Microsoft DI Docs](https://docs.microsoft.com/dotnet/core/extensions/dependency-injection)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Moq Documentation](https://github.com/moq/moq4)
- [xUnit Documentation](https://xunit.net/)

---

**Happy Coding! 🎉**
