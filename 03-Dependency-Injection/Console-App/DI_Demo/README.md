# Dependency Injection (DI) Demo Project

## 📚 Overview

This project demonstrates the concepts and benefits of **Dependency Injection (DI)** in .NET with practical examples and comprehensive unit tests.

## 🎯 What You'll Learn

1. **Problems with tight coupling** (code without DI)
2. **Benefits of loose coupling** (code with DI)
3. **How to implement DI** in .NET applications
4. **Different service lifetimes** (Singleton, Transient, Scoped)
5. **How DI improves testability** with comprehensive unit tests

## 📁 Project Structure

```
DI_Demo/
├── WithoutDI.cs           # Examples showing problems WITHOUT DI
├── WithDI.cs              # Examples showing benefits WITH DI
├── ServiceLifetimes.cs    # Comprehensive service lifetime examples
├── Program.cs             # Interactive demonstrations
├── README.md              # This file - project overview
├── SERVICE_LIFETIMES.md   # Complete service lifetime guide
└── ... (5 more documentation files)

DI_Demo.Tests/
├── WithoutDI_Tests.cs        # Tests showing difficulty without DI
├── WithDI_Tests.cs           # Tests showing ease with DI
└── ServiceLifetime_Tests.cs  # Service lifetime behavior tests
```

## 🚀 Running the Demo

### Run the Console Application

```bash
cd DI_Demo
dotnet run
```

The application will guide you through **five parts**:
1. **Part 1**: WITHOUT DI - Tight Coupling Problems
2. **Part 2**: WITH DI - Loose Coupling Benefits
3. **Part 3**: Service Lifetimes - Basic Demonstration
4. **Part 4**: Service Lifetimes - Real-World Examples
5. **Part 5**: Service Lifetimes - Scopes (Simulating Web Requests)

### Run the Tests

```bash
cd DI_Demo.Tests
dotnet test
```

**33 tests** demonstrating:
- Testing difficulties WITHOUT DI (3 tests)
- Testing ease WITH DI (13 tests)
- Service lifetime behaviors (17 tests)

## 💡 Key Concepts Explained

### Without DI (Tight Coupling)

**Problems:**
- ❌ Services create their own dependencies
- ❌ Hard to test (cannot mock dependencies)
- ❌ Hard to change (need to modify classes)
- ❌ Tight coupling to concrete implementations
- ❌ Violates SOLID principles

**Example:**
```csharp
public class UserService
{
    private readonly EmailService _emailService;
    
    public UserService()
    {
        // Hard-coded dependency - BAD!
        _emailService = new EmailService();
    }
}
```

### With DI (Loose Coupling)

**Benefits:**
- ✅ Services receive dependencies through constructor
- ✅ Easy to test (can inject mocks)
- ✅ Easy to change (swap implementations)
- ✅ Loose coupling via interfaces
- ✅ Follows SOLID principles

**Example:**
```csharp
public class UserService
{
    private readonly INotificationService _notificationService;
    
    // Dependency injected through constructor - GOOD!
    public UserService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
}
```

## 🔧 DI Configuration

```csharp
var services = new ServiceCollection();

// Register services with their interfaces
services.AddTransient<INotificationService, EmailNotificationService>();
services.AddTransient<UserService>();

// Build the container
var serviceProvider = services.BuildServiceProvider();

// Get service with dependencies auto-injected
var userService = serviceProvider.GetRequiredService<UserService>();
```

## 📊 Service Lifetimes

| Lifetime | Description | Use Cases | File Examples |
|----------|-------------|-----------|---------------|
| **Singleton** | Single instance for entire app lifetime | Configuration, Caching, Logging | [ServiceLifetimes.cs](ServiceLifetimes.cs) - AppConfiguration, CacheService |
| **Transient** | New instance every time requested | Lightweight stateless services | [ServiceLifetimes.cs](ServiceLifetimes.cs) - EmailValidator, PriceCalculator |
| **Scoped** | One instance per scope (e.g., per HTTP request) | Database contexts, per-request services | [ServiceLifetimes.cs](ServiceLifetimes.cs) - DatabaseContext, RequestContext |

### 📖 Learn More
See [SERVICE_LIFETIMES.md](SERVICE_LIFETIMES.md) for comprehensive guide with examples, diagrams, and best practices.

## 🧪 Testing with DI

### Without DI - Hard to Test
```csharp
[Fact]
public void Test_WithoutDI()
{
    var service = new UserService(); // Creates real EmailService internally
    
    // Cannot verify if email was sent
    // Cannot mock EmailService
    // Test is weak!
    service.RegisterUser("test", "test@example.com");
}
```

### With DI - Easy to Test
```csharp
[Fact]
public void Test_WithDI()
{
    // Create mock
    var mockNotification = new Mock<INotificationService>();
    
    // Inject mock
    var service = new UserService(mockNotification.Object);
    
    // Execute
    service.RegisterUser("test", "test@example.com");
    
    // Verify behavior
    mockNotification.Verify(
        x => x.SendNotification("test@example.com", "Welcome test!"),
        Times.Once
    );
}
```

## 📖 Real-World Scenarios Demonstrated

1. **User Registration** - Send welcome notification
2. **Order Processing** - Send order confirmation
3. **Payment Processing** - Log and notify
4. **Multiple Implementations** - Email, SMS, Push notifications
5. **Service Lifetimes** - Singleton vs Transient behavior

## 🎓 Best Practices

1. **Depend on abstractions (interfaces)**, not concrete classes
2. **Use constructor injection** for required dependencies
3. **Validate dependencies** (throw ArgumentNullException if null)
4. **Choose appropriate lifetime** for each service
5. **Register services in composition root** (startup/configuration)
6. **Keep interfaces focused** (Interface Segregation Principle)

## 🔍 What to Look For

When running the demo, notice:

1. **Part 1**: Services are tightly coupled, hard to change
2. **Part 2**: Same services, easily swapping implementations
3. **Part 3**: Different instances created based on lifetime
4. **Tests**: Mock verification and behavior testing with DI

## 📚 Further Reading

- [Microsoft DI Documentation](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Dependency Inversion Principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle)
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

## 🏆 Key Takeaways

1. **DI promotes loose coupling** - easier to maintain and extend
2. **DI improves testability** - can inject mocks for testing
3. **DI enables flexibility** - swap implementations without code changes
4. **DI follows SOLID principles** - especially Dependency Inversion
5. **DI is built into .NET** - use `Microsoft.Extensions.DependencyInjection`

## 📝 Assignment Ideas

Try these exercises to deepen your understanding:

1. Add a new notification service (e.g., Slack, Teams)
2. Create a service with Scoped lifetime and test it
3. Add error handling when notification service fails
4. Create a composite notification service (sends to multiple channels)
5. Add more unit tests for edge cases

---

**Happy Learning! 🎉**
