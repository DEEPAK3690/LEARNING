# ASP.NET Core Service Lifetimes

This document summarizes service lifetimes in ASP.NET Core with real-time examples.

---

## Table of Contents

1. [Service Lifetime Overview](#service-lifetime-overview)
2. [Transient](#transient)
3. [Scoped](#scoped)
4. [Singleton](#singleton)
5. [Comparison Table](#comparison-table)
6. [Parent Dependency Considerations](#parent-dependency-considerations)
7. [Common Mistakes](#common-mistakes)
8. [Real-Life Analogy](#real-life-analogy)
9. [References](#references)

---

## Service Lifetime Overview

ASP.NET Core provides three service lifetimes:

- **Transient**: Created every time requested
- **Scoped**: Created once per HTTP request
- **Singleton**: Created once for the entire application

---

## Transient

### `AddTransient`

**Characteristics:**

- **Created:** Every time requested
- **Disposed:** When the consuming object is disposed (or GC collects it if parent is transient)
- **Use case:** Lightweight, stateless services

### Example:

```csharp
builder.Services.AddTransient<IEmailService, EmailService>();
```

**Behavior:**

- `EmailService` is created every time `OrderService` asks for it
- Multiple requests create multiple instances

### Real-Time Flow:

```
HTTP Request Start
    |
    └── Create OrderService (Scoped)
        |
        └── Create EmailService (Transient)
            |
            └── Use EmailService
                |
HTTP Request End
    |
    ├── Dispose OrderService
    └── Dispose EmailService
```

---

## Scoped

### `AddScoped`

**Characteristics:**

- **Created:** Once per HTTP request
- **Shared:** Across services within the same request
- **Disposed:** At the end of the request
- **Use case:** Business logic, DbContext

### Example:

```csharp
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<AppDbContext>();
```

**Behavior:**

- One `OrderService` instance per request
- Shared `DbContext` for all services in request

---

## Singleton

### `AddSingleton`

**Characteristics:**

- **Created:** Once for the entire application
- **Shared:** Across all requests and users
- **Disposed:** When app shuts down
- **Use case:** Caching, configuration, shared data

### Example:

```csharp
builder.Services.AddSingleton<IProductCache, ProductCache>();
```

**Behavior:**

- `ProductCache` is the same for all requests

---

## Comparison Table

| Lifetime  | Created          | Shared Across       | Disposed             | Use Case                  |
| --------- | ---------------- | ------------------- | -------------------- | ------------------------- |
| Transient | Every time       | Not shared          | When parent disposed | Lightweight, stateless    |
| Scoped    | Once per request | Within same request | End of request       | Business logic, DbContext |
| Singleton | Once per app     | All requests/users  | App shutdown         | Caching, configuration    |

---

## Parent Dependency Considerations

Understanding how parent service lifetime affects child dependencies:

| Parent Service | Child Service | Disposal Behavior                                     |
| -------------- | ------------- | ----------------------------------------------------- |
| Scoped         | Transient     | Transient disposed at end of request                  |
| Transient      | Transient     | Transient disposed when GC collects it                |
| Singleton      | Transient     | Transient disposed at app shutdown ⚠️ (problematic) |

> **Warning:** Injecting shorter-lived services into longer-lived services can cause issues.

---

## Common Mistakes

❌ **Injecting Scoped into Singleton**

- Causes lifetime mismatch
- Scoped service may be disposed while Singleton still holds reference

❌ **Using heavy objects as Transient**

- Creates unnecessary GC pressure
- Performance degradation

❌ **Using DbContext as Transient**

- Multiple instances created
- Inconsistent data and tracking issues

---

## Real-Life Analogy

Think of service lifetimes like a store:

| Lifetime  | Analogy                                     |
| --------- | ------------------------------------------- |
| Singleton | Store inventory (shared for all customers)  |
| Scoped    | Cashier per customer (one per transaction)  |
| Transient | Receipt printer (new receipt for every use) |

---

## References

- [Microsoft Docs: Dependency Injection in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- Real-time e-commerce example: Order processing, EmailService, ProductCache
