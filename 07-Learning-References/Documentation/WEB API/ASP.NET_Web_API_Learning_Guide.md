
# 🧠 ASP.NET Web API – Complete Beginner Learning Guide

---

## 🔹 1. What is Web API?

**ASP.NET Web API** is a framework for building **HTTP-based services** that can be consumed by **browsers, mobile apps, or other servers**.

A **Web API (Web Application Programming Interface)** acts as a **bridge** between two systems — allowing them to communicate over the internet using **HTTP**.

Instead of connecting directly to databases, APIs define:
- **Rules** – what actions clients can perform  
- **Protocols** – how data is exchanged  
- **Formats** – how data is structured (usually JSON or XML)

### ✅ Why Web API
- To expose data and functionality securely over HTTP.  
- Ideal for **RESTful** services (stateless, simple, platform-independent).  
- Enables **cross-platform** communication (e.g., React app calling .NET API).

---

## 🔹 2. HTTP vs HTTPS

### **HTTP (HyperText Transfer Protocol)**
- The backbone of data communication on the web.  
- Transfers data as **plain text** (not secure).

### **HTTPS (HyperText Transfer Protocol Secure)**
- Secure version of HTTP using **SSL/TLS encryption**.  
- Encrypts sensitive data like passwords, tokens, or credit card info before transmission.

| Feature | HTTP | HTTPS |
|----------|------|--------|
| Security | ❌ Not encrypted | ✅ Encrypted (SSL/TLS) |
| Port | 80 | 443 |
| Certificate | Not required | Required |
| Use Case | Development | Production / Sensitive data |

### 💡 Real-time Example
If your login API sends data like:
```json
{ "username": "deepak", "password": "12345" }
```
- Under **HTTP**, it’s visible in plain text to attackers.
- Under **HTTPS**, it’s encrypted, unreadable to outsiders.

In ASP.NET Core:
```csharp
app.UseHttpsRedirection(); // Redirects all HTTP traffic to HTTPS
```

---

## 🔹 3. HTTP Requests and Responses

### **HTTP Request**
Message sent from **client → server** asking for a specific action.

#### 🧩 Components:
| Part | Description |
|------|--------------|
| Request Line | HTTP Method + URL + Version |
| Headers | Metadata (e.g. Authorization, Content-Type) |
| Body | Optional data (used in POST/PUT) |

#### 📘 Example:
```
POST /api/products HTTP/1.1
Host: www.myapi.com
Content-Type: application/json
Authorization: Bearer <token>

{
  "name": "Laptop",
  "price": 55000
}
```

---

### **HTTP Response**
Message sent from **server → client** after processing a request.

#### 🧩 Components:
| Part | Description |
|------|--------------|
| Status Line | HTTP version + status code + message |
| Headers | Info about response (content type, caching) |
| Body | Actual data (JSON, XML, etc.) |

#### 📘 Example:
```
HTTP/1.1 201 Created
Content-Type: application/json

{
  "id": 101,
  "name": "Laptop",
  "price": 55000
}
```

---

### **Common HTTP Status Codes**

| Code | Meaning | Usage |
|------|----------|--------|
| 200 | OK | Request successful |
| 201 | Created | Resource added |
| 204 | No Content | Deleted successfully |
| 400 | Bad Request | Invalid input |
| 401 | Unauthorized | Token missing/invalid |
| 404 | Not Found | Resource missing |
| 500 | Internal Server Error | Server crash/bug |

---

## 🔹 4. HTTP Methods (CRUD Operations)

| HTTP Method | Action | Example Endpoint | Description |
|--------------|---------|------------------|--------------|
| GET | Read | `/api/products` | Get all or one resource |
| POST | Create | `/api/products` | Add new resource |
| PUT | Update | `/api/products/5` | Update existing resource |
| DELETE | Delete | `/api/products/5` | Remove resource |

### Example:
```csharp
[HttpGet]       public IEnumerable<Product> Get() => _context.Products.ToList();
[HttpPost]      public IActionResult Post(Product p) { _context.Add(p); _context.SaveChanges(); return Created(); }
[HttpPut("{id}") ] public IActionResult Put(int id, Product p) { ... }
[HttpDelete("{id}")] public IActionResult Delete(int id) { ... }
```

---

## 🔹 5. Routing in Web API

Routing determines **which controller and method** handle a request.

### 🧩 Types of Routing

| Type | Defined | Example |
|------|----------|----------|
| Convention-based | In `WebApiConfig.cs` | `/api/{controller}/{id}` |
| Attribute-based | Above controller/action | `[Route("api/products/{id}")]` |

### Example:
```csharp
[Route("api/products")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public string GetById(int id) => $"Product ID: {id}";
}
```
🧭 URL → `/api/products/10`  
📤 Response → `Product ID: 10`

---

## 🔹 6. Controllers in Web API

Controllers handle incoming HTTP requests and return responses.

### Types:
- **ControllerBase** → For APIs (returns JSON/XML only)
- **Controller** → For MVC (returns Views + JSON)

### Example:
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new[] { "Pen", "Pencil" });
}
```

---

## 🔹 7. Models and DTOs

### **Model**
Defines the data structure (like a database table).

### **DTO (Data Transfer Object)**
Used to expose **only required** data to clients.

### Example:
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal CostPrice { get; set; }
    public string Supplier { get; set; }  // Hide this from client
}

public class ProductDto
{
    public string Name { get; set; }
    public decimal CostPrice { get; set; }
}
```

- ✅ **With DTO:** Sends safe, clean data  
- ❌ **Without DTO:** May expose sensitive fields

---

## 🔹 8. Dependency Injection (DI)

Used to achieve **loose coupling** between components.

### Example:
```csharp
builder.Services.AddScoped<IProductService, ProductService>();

public class ProductService : IProductService
{
    public string GetData() => "Injected dependency works!";
}
```

---

## 🔹 9. Database Connection using Entity Framework Core

1️⃣ Install EF Core packages  
2️⃣ Define `DbContext`  
3️⃣ Add Connection String in `appsettings.json`  
4️⃣ Perform CRUD using LINQ

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
}
```

---

## 🔹 10. Error Handling

### Example:
```csharp
[HttpGet("{id}")]
public IActionResult Get(int id)
{
    var product = _context.Products.Find(id);
    if (product == null) return NotFound("Product not found");
    return Ok(product);
}
```

For global error handling:
```csharp
app.UseExceptionHandler("/error");
```

---

## 🔹 11. Swagger (API Testing Tool)

Swagger automatically generates **API documentation and testing UI**.

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

app.UseSwagger();
app.UseSwaggerUI();
```

URL → `/swagger/index.html`

---

## 🔹 12. Authentication & Authorization

Use **JWT (JSON Web Tokens)** for secure access.

### Example:
```csharp
[Authorize]
[HttpGet("secure")]
public IActionResult SecureEndpoint() => Ok("This is protected");
```

---

## 🔹 13. CORS (Cross-Origin Resource Sharing)

Allows frontend (like Angular/React) to call your API.

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

app.UseCors("AllowAll");
```

---

## 🔹 14. Real-time Example Scenario

### 🛒 E-Commerce Web API

| API | Method | Description |
|-----|---------|-------------|
| `/api/products` | GET | List all products |
| `/api/products/{id}` | GET | Get product details |
| `/api/products` | POST | Add new product |
| `/api/products/{id}` | PUT | Update product |
| `/api/products/{id}` | DELETE | Delete product |

Frontend (React, Angular, etc.) calls these APIs via **HTTP requests**, gets **JSON responses**, and displays them to users.

---

## 🔹 15. Interview Tip Summary

| Topic | Key Question | Quick Answer |
|--------|---------------|--------------|
| Web API vs MVC | MVC returns View, API returns Data | API = JSON/XML only |
| HTTP Methods | GET/POST/PUT/DELETE | Map to CRUD |
| ControllerBase | API base class | No UI rendering |
| DTO | Hide sensitive data | Safer than exposing Model |
| HTTPS | Encrypted communication | Use in production |
| Dependency Injection | Loose coupling | AddScoped / AddTransient |
| Routing | URL mapping | Attribute routing is preferred |
| Swagger | API testing | `/swagger` endpoint |

---
