# ASP.NET Web API — Interview Cheat Sheet

---

## Core Concepts

### What is Web API?
- HTTP-based service framework built on ASP.NET Core
- Returns **JSON/XML** (not views like MVC)
- RESTful by convention — maps HTTP verbs to actions

### REST Principles (know these cold)
| Principle | Meaning |
|---|---|
| Stateless | Server stores no client state between requests |
| Uniform Interface | Standard HTTP verbs + resource URIs |
| Client-Server | Decoupled — client and server evolve independently |
| Cacheable | Responses declare if they're cacheable |
| Layered System | Client doesn't know if it's talking to a proxy or origin |

---

## HTTP Verbs

| Verb | Use | Idempotent? | Safe? |
|---|---|---|---|
| GET | Read | Yes | Yes |
| POST | Create | No | No |
| PUT | Full update/replace | Yes | No |
| PATCH | Partial update | No | No |
| DELETE | Delete | Yes | No |

> **Idempotent** = same request N times = same result as 1 time.

---

## Routing

```csharp
// Attribute routing (preferred)
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]           // GET api/products
    [HttpGet("{id}")]   // GET api/products/5
    [HttpPost]          // POST api/products
    [HttpPut("{id}")]   // PUT api/products/5
    [HttpDelete("{id}")] // DELETE api/products/5
}
```

- `[ApiController]` enables automatic model validation (400 on bad input), binding inference
- `ControllerBase` — no view support (lighter than `Controller`)

---

## Model Binding & Validation

```csharp
// Binding sources
[FromBody]   // JSON body (default for complex types with [ApiController])
[FromRoute]  // Route param
[FromQuery]  // ?key=value
[FromHeader] // Request header
[FromForm]   // Form data

// Validation
public class ProductDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Range(0.01, 10000)]
    public decimal Price { get; set; }
}
```

`[ApiController]` automatically returns `400 Bad Request` if `ModelState` is invalid — no need to check manually.

---

## Action Results

```csharp
// Preferred: IActionResult or ActionResult<T>
public ActionResult<Product> Get(int id)
{
    var product = _repo.Find(id);
    if (product == null) return NotFound();      // 404
    return Ok(product);                          // 200
}

// Common helpers
return Ok(data);             // 200
return Created(uri, data);   // 201
return NoContent();          // 204
return BadRequest();         // 400
return Unauthorized();       // 401
return Forbid();             // 403
return NotFound();           // 404
return StatusCode(500, msg); // custom
```

---

## Dependency Injection (DI)

```csharp
// Program.cs registration
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
builder.Services.AddSingleton<ICache, MemoryCache>();

// Constructor injection
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    public ProductsController(IProductService service) => _service = service;
}
```

| Lifetime | Created | Shared |
|---|---|---|
| **Transient** | Every injection | Never |
| **Scoped** | Per HTTP request | Within request |
| **Singleton** | Once per app lifetime | Always |

---

## Middleware Pipeline

```csharp
// Order matters!
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();  // Who are you?
app.UseAuthorization();   // What can you do?
app.MapControllers();
```

> Authentication must come **before** Authorization.

---

## Filters (run around action execution)

| Filter | When it runs |
|---|---|
| `IActionFilter` | Before/after action method |
| `IExceptionFilter` | On unhandled exception |
| `IAuthorizationFilter` | Before model binding |
| `IResultFilter` | Before/after result execution |

```csharp
public class LogActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext ctx) { /* before */ }
    public void OnActionExecuted(ActionExecutedContext ctx)  { /* after */  }
}
```

---

## Authentication & Authorization

```csharp
// JWT Bearer setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Key"]))
        };
    });

// Usage
[Authorize]                          // requires auth
[Authorize(Roles = "Admin")]         // role-based
[Authorize(Policy = "MinAge18")]     // policy-based
[AllowAnonymous]                     // bypass auth
```

---

## Global Exception Handling

```csharp
// Minimal API style (ASP.NET Core 8+)
app.UseExceptionHandler(appError =>
{
    appError.Run(async ctx =>
    {
        ctx.Response.StatusCode = 500;
        ctx.Response.ContentType = "application/json";
        var error = ctx.Features.Get<IExceptionHandlerFeature>();
        await ctx.Response.WriteAsJsonAsync(new { message = error?.Error.Message });
    });
});

// Or use a custom middleware / IExceptionFilter
```

---

## CORS

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

app.UseCors("AllowAll"); // before MapControllers
```

---

## Versioning

```csharp
// NuGet: Asp.Versioning.Mvc
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
});

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsV1Controller : ControllerBase { }
```

---

## Minimal APIs (ASP.NET Core 6+)

```csharp
var app = WebApplication.Create(args);

app.MapGet("/products", (IProductService svc) => svc.GetAll());
app.MapPost("/products", (ProductDto dto, IProductService svc) => {
    var product = svc.Create(dto);
    return Results.Created($"/products/{product.Id}", product);
});

app.Run();
```

Minimal APIs vs Controllers:
- Minimal = less ceremony, good for microservices/simple APIs
- Controllers = better for large, complex APIs (filters, model binding, conventions)

---

## Common Interview Questions

**Q: Difference between `IActionResult` and `ActionResult<T>`?**
`ActionResult<T>` gives you type safety + Swagger knows the response type. `IActionResult` is more flexible but loses type info.

**Q: What does `[ApiController]` do?**
1. Automatic 400 for invalid `ModelState`
2. Binding source inference (`[FromBody]`, `[FromRoute]`, etc.)
3. Problem details responses (RFC 7807)

**Q: PUT vs PATCH?**
PUT replaces the entire resource. PATCH applies partial changes. Use `JsonPatchDocument<T>` for PATCH in ASP.NET Core.

**Q: How do you secure an endpoint?**
JWT Bearer auth → `AddAuthentication` + `AddJwtBearer` → `[Authorize]` on controller/action.

**Q: What's the request pipeline order?**
Exception Handler → HTTPS Redirect → Static Files → Routing → **Authentication → Authorization** → Endpoints

**Q: Scoped vs Singleton — when does it matter?**
Never inject a Scoped service into a Singleton — the Scoped service becomes effectively singleton and can hold stale data or cause threading issues.

**Q: How do you return 201 Created with a Location header?**
```csharp
return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
```

**Q: What is content negotiation?**
Client sends `Accept: application/xml`, server returns XML if it can. ASP.NET Core does this automatically with `AddXmlSerializerFormatters()`.

**Q: How do you handle versioning?**
URL segment (`/api/v1/`), query string (`?api-version=1.0`), or header (`api-version: 1.0`). Use `Asp.Versioning.Mvc` package.

---

## Quick Checklist for Any API

- [ ] `[ApiController]` + `ControllerBase`
- [ ] Attribute routing
- [ ] DTOs (never expose EF entities directly)
- [ ] `ActionResult<T>` return types
- [ ] Global exception handler
- [ ] Validation attributes + auto-400
- [ ] JWT auth if secured
- [ ] CORS configured
- [ ] Swagger (`AddEndpointsApiExplorer` + `AddSwaggerGen`)
- [ ] `ILogger<T>` injected for logging
