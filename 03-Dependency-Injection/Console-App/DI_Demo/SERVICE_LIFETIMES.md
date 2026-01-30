# 🔄 Service Lifetimes in .NET Dependency Injection - Complete Guide

## 📚 Table of Contents
1. [Overview](#overview)
2. [The Three Lifetimes](#the-three-lifetimes)
3. [Transient Lifetime](#transient-lifetime)
4. [Scoped Lifetime](#scoped-lifetime)
5. [Singleton Lifetime](#singleton-lifetime)
6. [Real-World Examples](#real-world-examples)
7. [Choosing the Right Lifetime](#choosing-the-right-lifetime)
8. [Common Pitfalls](#common-pitfalls)
9. [Best Practices](#best-practices)

---

## Overview

**Service Lifetime** determines how long a service instance lives and when it gets created/destroyed in the DI container.

### Why It Matters

```
❌ Wrong lifetime choice:
   - Memory leaks (Singleton holding onto resources)
   - Performance issues (Creating expensive objects too often)
   - Bugs (Shared state when it shouldn't be)

✅ Right lifetime choice:
   - Optimal performance
   - Proper resource management
   - Correct behavior
```

---

## The Three Lifetimes

| Lifetime | When Created | When Disposed | Use Case |
|----------|--------------|---------------|----------|
| **Transient** | Every request | Immediately after use | Lightweight, stateless |
| **Scoped** | Once per scope | End of scope | Per-request data |
| **Singleton** | Once per app | App shutdown | Expensive, shared |

### Visual Representation

```
APPLICATION LIFETIME
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│  Singleton ████████████████████████████████████████████    │ One instance
│                                                             │
│  ┌─────────────────────────────────────────────────┐      │
│  │ SCOPE 1 (e.g., HTTP Request 1)                  │      │
│  │                                                  │      │
│  │  Scoped    ████████████████████████████         │      │ One per scope
│  │                                                  │      │
│  │  Transient █  █  █  █  █  █  █  █  █  █  █     │      │ New each time
│  │                                                  │      │
│  └─────────────────────────────────────────────────┘      │
│                                                             │
│  ┌─────────────────────────────────────────────────┐      │
│  │ SCOPE 2 (e.g., HTTP Request 2)                  │      │
│  │                                                  │      │
│  │  Scoped    ████████████████████████████         │      │ New instance
│  │                                                  │      │
│  │  Transient █  █  █  █  █  █  █  █  █  █  █     │      │ New each time
│  │                                                  │      │
│  └─────────────────────────────────────────────────┘      │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Transient Lifetime

### Concept
**A new instance is created every time the service is requested.**

### Registration
```csharp
services.AddTransient<IEmailValidator, EmailValidator>();
services.AddTransient<PriceCalculator>();
```

### Behavior
```csharp
var validator1 = serviceProvider.GetService<IEmailValidator>();
var validator2 = serviceProvider.GetService<IEmailValidator>();

// validator1 != validator2 (different instances)
```

### When to Use
✅ **Use Transient for:**
- Lightweight services
- Stateless operations
- Services that don't hold resources
- Validators, calculators, converters
- Services with no expensive initialization

❌ **Don't Use Transient for:**
- Services with expensive initialization
- Services that hold resources (connections, file handles)
- Services that need to maintain state

### Real-World Examples

```csharp
// ✅ GOOD - Lightweight validator
public class EmailValidator
{
    public bool IsValid(string email)
    {
        return !string.IsNullOrEmpty(email) && email.Contains("@");
    }
}

// ✅ GOOD - Stateless calculator
public class TaxCalculator
{
    public decimal Calculate(decimal amount, decimal rate)
    {
        return amount * rate;
    }
}

// ❌ BAD - Expensive to create (use Singleton instead)
public class HeavyService
{
    public HeavyService()
    {
        // Loads 100MB configuration file
        // Creates network connections
        // Initializes expensive resources
    }
}
```

### Memory Impact
```
10 requests with Transient:
┌────┐ ┌────┐ ┌────┐ ┌────┐ ┌────┐
│ 1  │ │ 2  │ │ 3  │ │ 4  │ │ 5  │  = 10 instances created
└────┘ └────┘ └────┘ └────┘ └────┘
┌────┐ ┌────┐ ┌────┐ ┌────┐ ┌────┐
│ 6  │ │ 7  │ │ 8  │ │ 9  │ │ 10 │
└────┘ └────┘ └────┘ └────┘ └────┘

Impact: More instances, more GC pressure
Good for: Lightweight services
```

---

## Scoped Lifetime

### Concept
**One instance is created per scope. In web apps, typically one instance per HTTP request.**

### Registration
```csharp
services.AddScoped<IDbContext, AppDbContext>();
services.AddScoped<RequestContext>();
```

### Behavior
```csharp
using (var scope = serviceProvider.CreateScope())
{
    var db1 = scope.ServiceProvider.GetService<IDbContext>();
    var db2 = scope.ServiceProvider.GetService<IDbContext>();
    
    // db1 == db2 (same instance within scope)
}

using (var scope2 = serviceProvider.CreateScope())
{
    var db3 = scope2.ServiceProvider.GetService<IDbContext>();
    
    // db3 != db1 (different scope, different instance)
}
```

### When to Use
✅ **Use Scoped for:**
- Database contexts (Entity Framework DbContext)
- Unit of Work pattern
- Request-specific data (user info, correlation ID)
- Services that need to maintain state during a request
- Services that need to be consistent within a request

❌ **Don't Use Scoped for:**
- Services in non-web applications (no natural scope)
- Services that need to live beyond request lifetime
- Services shared across all requests

### Real-World Examples

```csharp
// ✅ GOOD - Database context (per request)
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    // Should be disposed at end of request
}

// ✅ GOOD - Request context
public class RequestContext
{
    public string UserId { get; set; }
    public string CorrelationId { get; set; }
    public DateTime RequestTime { get; set; }
}

// ✅ GOOD - Unit of Work
public class UnitOfWork
{
    private readonly AppDbContext _context;
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    
    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
```

### Web Application Flow

```
HTTP REQUEST 1
┌──────────────────────────────────────────────────────┐
│ Scope Created                                        │
│                                                      │
│  Controller → Service → Repository                  │
│      ↓           ↓           ↓                       │
│      └───────────┴───────────┴─→ SAME DbContext    │
│                                                      │
│ Scope Disposed (DbContext.Dispose() called)         │
└──────────────────────────────────────────────────────┘

HTTP REQUEST 2
┌──────────────────────────────────────────────────────┐
│ Scope Created                                        │
│                                                      │
│  Controller → Service → Repository                  │
│      ↓           ↓           ↓                       │
│      └───────────┴───────────┴─→ NEW DbContext      │
│                                                      │
│ Scope Disposed (DbContext.Dispose() called)         │
└──────────────────────────────────────────────────────┘
```

---

## Singleton Lifetime

### Concept
**A single instance is created once and shared throughout the entire application lifetime.**

### Registration
```csharp
services.AddSingleton<IConfiguration, AppConfiguration>();
services.AddSingleton<ICacheService, CacheService>();
```

### Behavior
```csharp
var config1 = serviceProvider.GetService<IConfiguration>();
var config2 = serviceProvider.GetService<IConfiguration>();

// config1 == config2 (same instance always)

using (var scope1 = serviceProvider.CreateScope())
{
    var config3 = scope1.ServiceProvider.GetService<IConfiguration>();
    // config3 == config1 (same instance across scopes)
}
```

### When to Use
✅ **Use Singleton for:**
- Configuration settings
- Caching services
- Logging services
- Services expensive to create
- Services with no state or thread-safe shared state
- Factory services

❌ **Don't Use Singleton for:**
- Services that hold per-request data
- Services that are not thread-safe
- Services that shouldn't share state

⚠️ **IMPORTANT: Singletons MUST be thread-safe!**

### Real-World Examples

```csharp
// ✅ GOOD - Configuration (loaded once)
public class AppConfiguration
{
    private readonly Dictionary<string, string> _settings;
    
    public AppConfiguration()
    {
        // Expensive: Load from file/database
        _settings = LoadSettings();
    }
    
    public string GetSetting(string key) => _settings[key];
}

// ✅ GOOD - Cache service (shared)
public class CacheService
{
    private readonly ConcurrentDictionary<string, object> _cache = new();
    
    public void Set(string key, object value)
    {
        _cache[key] = value;
    }
    
    public T Get<T>(string key)
    {
        _cache.TryGetValue(key, out var value);
        return (T)value;
    }
}

// ✅ GOOD - Logger (shared, thread-safe)
public class Logger : ILogger
{
    private readonly object _lock = new object();
    
    public void Log(string message)
    {
        lock (_lock) // Thread-safe
        {
            File.AppendAllText("log.txt", message);
        }
    }
}

// ❌ BAD - Not thread-safe (use Scoped instead)
public class RequestData
{
    public string UserId { get; set; } // Shared across all requests! BAD!
}
```

### Memory Impact
```
10 requests with Singleton:
┌────┐
│ 1  │  ← Same instance for all requests
└────┘

Impact: Minimal memory, excellent performance
Good for: Expensive-to-create, shared services
⚠️ Must be: Thread-safe!
```

---

## Real-World Examples

### Example 1: E-Commerce Application

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // SINGLETON - App-wide services
    services.AddSingleton<IConfiguration, AppConfiguration>();
    services.AddSingleton<ICache, RedisCache>();
    services.AddSingleton<ILogger, FileLogger>();
    
    // SCOPED - Per-request services
    services.AddScoped<IDbContext, ECommerceDbContext>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IUserContext, UserContext>();
    
    // TRANSIENT - Lightweight operations
    services.AddTransient<IEmailValidator, EmailValidator>();
    services.AddTransient<IPriceCalculator, PriceCalculator>();
    services.AddTransient<IProductMapper, ProductMapper>();
}
```

### Example 2: Service Dependencies

```csharp
// Singleton depends on Singleton ✅
public class Logger
{
    private readonly IConfiguration _config; // Singleton
    
    public Logger(IConfiguration config) { }
}

// Scoped depends on Singleton ✅
public class OrderService
{
    private readonly ILogger _logger;        // Singleton
    private readonly IDbContext _db;         // Scoped
    
    public OrderService(ILogger logger, IDbContext db) { }
}

// Transient depends on anything ✅
public class PriceCalculator
{
    private readonly IConfiguration _config;  // Singleton
    private readonly ITaxService _taxService; // Scoped or Transient
    
    public PriceCalculator(IConfiguration config, ITaxService taxService) { }
}

// ❌ CAPTIVE DEPENDENCY - BAD!
public class SingletonService
{
    private readonly IScopedService _scoped; // ❌ Singleton capturing Scoped
    
    public SingletonService(IScopedService scoped)
    {
        // PROBLEM: Scoped service becomes a singleton!
        // It will never be disposed until app shutdown
        _scoped = scoped;
    }
}
```

---

## Choosing the Right Lifetime

### Decision Tree

```
Is the service expensive to create?
│
├─ YES → Is it thread-safe or stateless?
│        │
│        ├─ YES → SINGLETON ✅
│        │
│        └─ NO → SCOPED (for web apps)
│
└─ NO → Does it need per-request state?
         │
         ├─ YES → SCOPED ✅
         │
         └─ NO → TRANSIENT ✅
```

### Quick Reference Table

| Service Type | Lifetime | Reason |
|--------------|----------|--------|
| Configuration | Singleton | Loaded once, shared |
| Cache | Singleton | Shared across app, thread-safe |
| Logger | Singleton | Shared, thread-safe, expensive I/O |
| DbContext (EF) | Scoped | Per-request, disposable |
| Unit of Work | Scoped | Per-request transaction |
| User Context | Scoped | Request-specific data |
| Validators | Transient | Lightweight, stateless |
| Calculators | Transient | Lightweight, stateless |
| Mappers | Transient | Lightweight, stateless |
| Email Service | Scoped/Transient | Depends on implementation |

---

## Common Pitfalls

### 1. Captive Dependency ❌

```csharp
// ❌ PROBLEM
services.AddSingleton<MySingletonService>();  // Captures scoped dependency
services.AddScoped<MyScopedService>();

public class MySingletonService
{
    private readonly MyScopedService _scoped; // ❌ Captured!
    
    public MySingletonService(MyScopedService scoped)
    {
        _scoped = scoped; // This scoped service is now a singleton!
    }
}

// ✅ SOLUTION 1: Use IServiceProvider
public class MySingletonService
{
    private readonly IServiceProvider _provider;
    
    public MySingletonService(IServiceProvider provider)
    {
        _provider = provider;
    }
    
    public void DoWork()
    {
        using (var scope = _provider.CreateScope())
        {
            var scoped = scope.ServiceProvider.GetService<MyScopedService>();
            // Use scoped service
        }
    }
}

// ✅ SOLUTION 2: Change both to Scoped
services.AddScoped<MySingletonService>();  // Now scoped
services.AddScoped<MyScopedService>();
```

### 2. Memory Leaks with Singleton ❌

```csharp
// ❌ PROBLEM - Singleton holding references
public class CacheService
{
    private readonly List<byte[]> _data = new(); // Never cleared!
    
    public void Add(byte[] data)
    {
        _data.Add(data); // Memory leak - data never removed
    }
}

// ✅ SOLUTION - Implement eviction
public class CacheService
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    
    public void Set(string key, object value, TimeSpan expiration)
    {
        _cache[key] = new CacheEntry 
        { 
            Value = value, 
            ExpiresAt = DateTime.UtcNow.Add(expiration) 
        };
        
        CleanupExpired(); // Remove old entries
    }
}
```

### 3. Thread Safety Issues ❌

```csharp
// ❌ PROBLEM - Singleton not thread-safe
public class Counter
{
    private int _count = 0;
    
    public void Increment()
    {
        _count++; // Not thread-safe! Race condition!
    }
}

// ✅ SOLUTION - Use thread-safe operations
public class Counter
{
    private int _count = 0;
    
    public void Increment()
    {
        Interlocked.Increment(ref _count); // Thread-safe
    }
}
```

---

## Best Practices

### ✅ Do's

1. **Default to Transient** unless you have a reason
2. **Use Scoped for DbContext** (Entity Framework)
3. **Use Singleton for expensive services** (caching, configuration)
4. **Keep Singletons thread-safe**
5. **Dispose resources properly** (Scoped services auto-disposed)
6. **Document lifetime choices** in comments

### ❌ Don'ts

1. **Don't use Singleton for per-request data**
2. **Don't capture shorter-lived services** in longer-lived services
3. **Don't assume Transient is free** (GC pressure)
4. **Don't share mutable state** in Singletons without synchronization
5. **Don't use Scoped** in non-web apps without explicit scopes

---

## Testing Service Lifetimes

### How to Verify Behavior

```csharp
[Fact]
public void TransientService_CreatesNewInstance()
{
    var services = new ServiceCollection();
    services.AddTransient<MyService>();
    var provider = services.BuildServiceProvider();
    
    var instance1 = provider.GetService<MyService>();
    var instance2 = provider.GetService<MyService>();
    
    Assert.NotSame(instance1, instance2); // Different instances
}

[Fact]
public void ScopedService_SameWithinScope_DifferentAcrossScopes()
{
    var services = new ServiceCollection();
    services.AddScoped<MyService>();
    var provider = services.BuildServiceProvider();
    
    using (var scope1 = provider.CreateScope())
    {
        var a = scope1.ServiceProvider.GetService<MyService>();
        var b = scope1.ServiceProvider.GetService<MyService>();
        Assert.Same(a, b); // Same within scope
    }
    
    using (var scope2 = provider.CreateScope())
    {
        var c = scope2.ServiceProvider.GetService<MyService>();
        // c is different from a and b (different scope)
    }
}

[Fact]
public void SingletonService_AlwaysSame()
{
    var services = new ServiceCollection();
    services.AddSingleton<MyService>();
    var provider = services.BuildServiceProvider();
    
    var instance1 = provider.GetService<MyService>();
    var instance2 = provider.GetService<MyService>();
    
    using (var scope = provider.CreateScope())
    {
        var instance3 = scope.ServiceProvider.GetService<MyService>();
        Assert.Same(instance1, instance3); // Always same
    }
}
```

---

## Summary

```
┌──────────────┬────────────────┬─────────────────┬──────────────────┐
│ Lifetime     │ Instance Count │ Disposal        │ Best For         │
├──────────────┼────────────────┼─────────────────┼──────────────────┤
│ Transient    │ Many           │ After use       │ Lightweight ops  │
│ Scoped       │ One per scope  │ End of scope    │ Per-request data │
│ Singleton    │ One per app    │ App shutdown    │ Shared resources │
└──────────────┴────────────────┴─────────────────┴──────────────────┘
```

### Key Takeaways

1. ⚡ **Transient** = New every time (good for lightweight)
2. 🔄 **Scoped** = One per request (good for DbContext)
3. 🌍 **Singleton** = One forever (good for config/cache)
4. ⚠️ **Thread Safety** = Critical for Singletons
5. ❌ **Captive Dependencies** = Avoid mixing lifetimes incorrectly

---

**Choose wisely, code safely! 🚀**
