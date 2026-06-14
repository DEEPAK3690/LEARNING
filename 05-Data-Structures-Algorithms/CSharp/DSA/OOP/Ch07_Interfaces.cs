// ============================================================
// FILE: Ch07_Interfaces.cs
// PURPOSE: Deep dive into interfaces — the backbone of flexible,
//          testable, extensible .NET architecture.
// ============================================================

namespace DSA.OOP
{
    // ─── LIVE DEMO INTERFACES ──────────────────────────────────────────

    // Repository pattern — abstracting data access behind an interface
    internal interface IProductRepository
    {
        Product2 GetById(int id);
        IEnumerable<Product2> GetAll();
        IEnumerable<Product2> Search(string query);
        void Add(Product2 product);
        void Update(Product2 product);
        bool Delete(int id);
    }

    // Simple product entity for the demo
    internal class Product2
    {
        public int    Id       { get; set; }
        public string Name     { get; set; }
        public decimal Price   { get; set; }
        public int    Stock    { get; set; }
        public override string ToString() => $"[{Id}] {Name} - ${Price:F2} (Stock: {Stock})";
    }

    // IN-MEMORY implementation (fast, for unit tests and development)
    // Implements BOTH IProductRepository AND IFullRepository (which extends IReadable/IWritable/ISearchable)
    // This shows a class can satisfy multiple interface contracts simultaneously.
    internal class InMemoryProductRepository : IProductRepository, IFullRepository
    {
        private readonly List<Product2> _products;
        private int _nextId = 1;

        public InMemoryProductRepository()
        {
            _products = new List<Product2>
            {
                new() { Id = _nextId++, Name = "USB-C Cable",     Price = 15.99m, Stock = 50 },
                new() { Id = _nextId++, Name = "HDMI Adapter",    Price = 24.99m, Stock = 30 },
                new() { Id = _nextId++, Name = "USB Hub 7-Port",  Price = 39.99m, Stock = 20 },
            };
        }

        public Product2 GetById(int id)          => _products.FirstOrDefault(p => p.Id == id);
        public IEnumerable<Product2> GetAll()    => _products.AsReadOnly();
        public IEnumerable<Product2> Search(string q) =>
            _products.Where(p => p.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
        public void Add(Product2 p)    { p.Id = _nextId++; _products.Add(p); }
        public void Update(Product2 p) { var i = _products.FindIndex(x => x.Id == p.Id); if (i >= 0) _products[i] = p; }
        public bool Delete(int id)     { return _products.RemoveAll(p => p.Id == id) > 0; }
    }

    // ISP DEMO: Segregated vs fat interfaces
    internal interface IReadable   { IEnumerable<Product2> GetAll(); Product2 GetById(int id); }
    internal interface IWritable   { void Add(Product2 p); void Update(Product2 p); bool Delete(int id); }
    internal interface ISearchable { IEnumerable<Product2> Search(string query); }

    // Full read-write access
    internal interface IFullRepository : IReadable, IWritable, ISearchable { }

    // Read-only service — only gets IReadable, can't accidentally write
    internal class ProductCatalogService
    {
        private readonly IReadable _reader; // only reading — safe, minimal surface area

        public ProductCatalogService(IReadable reader) => _reader = reader;

        public void PrintCatalog()
        {
            foreach (var p in _reader.GetAll())
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"     {p}");
            }
            Console.ResetColor();
        }
    }

    // ─── THE CHAPTER ───────────────────────────────────────────────────
    internal static class Ch07_Interfaces
    {
        public static void Run()
        {
            UI.Header("CHAPTER 7: INTERFACES — CONTRACTS WITHOUT IMPLEMENTATION");

            UI.Concept(
                "An interface is a PURE CONTRACT. It says: 'Any class claiming to be this type " +
                "MUST provide exactly these methods and properties.' " +
                "The interface makes no promises about HOW — only WHAT.\n\n" +
                "Interfaces are the backbone of professional .NET architecture. " +
                "Every time you see IRepository, ILogger, IService, IHandler — that's this."
            );
            UI.Pause();

            Section1_WhatInterfacesAre();
            Section2_WhyTheyMatter();
            Section3_MultipleImplementation();
            Section4_InterfaceSegregation();
            Section5_Repository();
            Section6_DependencyInjection();
            Section7_LiveDemo();
            RunQuiz();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section1_WhatInterfacesAre()
        {
            UI.SubHeader("What an Interface Is (and Isn't)");

            UI.Analogy("A Hiring Contract",
                "A job posting says: 'The candidate MUST be able to: speak English, " +
                "write C#, use Git, and work in a team.' It doesn't say HOW you learned these skills, " +
                "where you studied, or what tools you use. As long as you CAN do these things, " +
                "you satisfy the contract. An interface is exactly this — a set of requirements " +
                "any implementing class must fulfill."
            );

            UI.Code("Interface anatomy — every part explained",
                "// Naming convention: ALWAYS start with 'I'",
                "// This tells every developer: 'this is an interface, not a class'",
                "public interface ILogger",
                "{",
                "    // METHOD SIGNATURES: name + parameters + return type.",
                "    // NO body (no {}). NO implementation.",
                "    // Implementing classes MUST provide the body.",
                "    void Log(string message);",
                "    void LogError(string message, Exception exception);",
                "",
                "    // PROPERTY SIGNATURES: get/set accessors declared, not implemented.",
                "    bool IsEnabled { get; }",
                "    string Category { get; set; }",
                "",
                "    // DEFAULT METHODS (C# 8+): optional, can have a body.",
                "    // Implementing classes don't HAVE to override this.",
                "    // Use sparingly — this is for convenience, not core design.",
                "    void LogWarning(string message) => Log($\"[WARNING] {message}\");",
                "}",
                "",
                "// What interfaces CANNOT have:",
                "// - Fields (_myField) — no instance state",
                "// - Constructors — can't be instantiated directly",
                "// - Private instance methods (before C# 8)"
            );

            UI.Code("Implementing an interface",
                "// 'class ConsoleLogger : ILogger' promises to fulfill ALL of ILogger",
                "public class ConsoleLogger : ILogger",
                "{",
                "    // Required by ILogger: MUST implement or compile fails",
                "    public void Log(string message) => Console.WriteLine($\"[LOG] {message}\");",
                "    public void LogError(string msg, Exception ex)",
                "        => Console.WriteLine($\"[ERR] {msg}: {ex?.Message}\");",
                "",
                "    // Required by ILogger — the property:",
                "    public bool   IsEnabled { get; } = true;",
                "    public string Category  { get; set; } = \"Default\";",
                "",
                "    // LogWarning is optional (has default impl). We DON'T override it.",
                "    // It will use the interface's default implementation.",
                "}"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section2_WhyTheyMatter()
        {
            UI.SubHeader("Why Interfaces Matter — The Core Benefits");

            UI.Print("3 transformative benefits of interfaces:\n");

            UI.Print("BENEFIT 1: TESTABILITY");
            UI.Code("Without interface — hard to test",
                "// ❌ OrderService DEPENDS ON the concrete SqlDatabase class",
                "public class OrderService",
                "{",
                "    private SqlDatabase _db = new SqlDatabase(connectionString);",
                "    // To unit test: you MUST have a SQL Server running.",
                "    // Tests are slow, flaky, require infrastructure. Bad.",
                "}"
            );
            UI.Code("With interface — easy to test",
                "// ✓ OrderService DEPENDS ON IDatabase (an interface)",
                "public class OrderService",
                "{",
                "    private IDatabase _db;",
                "    public OrderService(IDatabase db) => _db = db;",
                "    // In production: inject SqlDatabase",
                "    // In unit tests:  inject FakeDatabase (returns predictable test data)",
                "    // Tests run in milliseconds, no infrastructure needed.",
                "}"
            );

            UI.Print("\nBENEFIT 2: FLEXIBILITY — SWAP IMPLEMENTATIONS FREELY");
            UI.Concept(
                "Your application starts with LocalFileLogger. Six months later, you need " +
                "AzureApplicationInsightsLogger. If your code depends on ILogger, the swap is:\n" +
                "  1. Write the new class\n  2. Change the registration in your DI container\n" +
                "  3. Done. Zero changes to business logic."
            );

            UI.Print("\nBENEFIT 3: CONTRACTS MAKE TEAMS MORE PRODUCTIVE");
            UI.Concept(
                "Team A defines IProductService interface. " +
                "Team B implements the service. Team C writes the UI that uses it. " +
                "All three teams work in PARALLEL once the interface is agreed on. " +
                "Teams B and C don't wait for each other — they work against the interface contract."
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section3_MultipleImplementation()
        {
            UI.SubHeader("Multiple Interface Implementation — A Class Can Have Many Roles");

            UI.Concept(
                "Unlike inheritance (one base class only), a class can implement MULTIPLE interfaces. " +
                "This is how C# achieves the benefits of multiple inheritance without its problems. " +
                "A class can wear many hats — each hat is an interface."
            );

            UI.Code("One class, three interfaces",
                "public interface ILoggable    { void Log(string message); }",
                "public interface ISaveable    { void Save(); bool Load(); }",
                "public interface IValidatable { bool Validate(); IEnumerable<string> GetErrors(); }",
                "",
                "// This class satisfies ALL three contracts:",
                "public class UserProfile : ILoggable, ISaveable, IValidatable",
                "{",
                "    public string Name  { get; set; }",
                "    public string Email { get; set; }",
                "",
                "    // From ILoggable:",
                "    public void Log(string msg) => Console.WriteLine($\"[User:{Name}] {msg}\");",
                "",
                "    // From ISaveable:",
                "    public void Save() => Log($\"Saving profile for {Email}\");",
                "    public bool Load() { Log(\"Loading profile...\"); return true; }",
                "",
                "    // From IValidatable:",
                "    public bool Validate() => !GetErrors().Any();",
                "    public IEnumerable<string> GetErrors()",
                "    {",
                "        if (string.IsNullOrWhiteSpace(Name))  yield return \"Name is required\";",
                "        if (!Email.Contains('@'))             yield return \"Invalid email format\";",
                "    }",
                "}",
                "",
                "// Now UserProfile can be used in ANY context that expects any of those interfaces:",
                "ILoggable    logger    = new UserProfile();  // valid",
                "ISaveable    saveable  = new UserProfile();  // valid",
                "IValidatable validator = new UserProfile();  // valid"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section4_InterfaceSegregation()
        {
            UI.SubHeader("Interface Segregation — Keep Interfaces Small and Focused");

            UI.Concept(
                "This is the 'I' in SOLID. The principle: 'Clients should not be forced to depend " +
                "on interfaces they do not use.' Break large, fat interfaces into smaller, " +
                "focused ones. Each interface should represent ONE cohesive set of operations."
            );

            UI.Code("Fat interface — forces implementers to stub things they don't support",
                "// ❌ BAD: Fat interface",
                "public interface IAnimal",
                "{",
                "    void Eat();",
                "    void Sleep();",
                "    void Fly();     // Dogs can't fly!",
                "    void Swim();    // Eagles can't swim!",
                "    void Bark();    // Eagles don't bark!",
                "}",
                "",
                "public class Dog : IAnimal",
                "{",
                "    public void Eat()   => Console.WriteLine(\"Dog eating\");",
                "    public void Sleep() => Console.WriteLine(\"Dog sleeping\");",
                "    public void Fly()   => throw new NotImplementedException(); // 💥 Can't fly",
                "    public void Swim()  => Console.WriteLine(\"Dog paddling\");",
                "    public void Bark()  => Console.WriteLine(\"Woof!\");",
                "}",
                "// Dog is forced to implement Fly() — it throws, which is a runtime trap."
            );

            UI.Code("Segregated interfaces — each focused on one capability",
                "// ✓ GOOD: Small, focused interfaces",
                "public interface IFeedable  { void Eat(); }",
                "public interface ISleepable { void Sleep(); }",
                "public interface IFlyable   { void Fly(); }",
                "public interface ISwimmable { void Swim(); }",
                "public interface IBarkable  { void Bark(); }",
                "",
                "// Each class implements only what MAKES SENSE for it:",
                "public class Dog   : IFeedable, ISleepable, ISwimmable, IBarkable { ... }",
                "public class Eagle : IFeedable, ISleepable, IFlyable              { ... }",
                "public class Fish  : IFeedable, ISleepable, ISwimmable            { ... }",
                "",
                "// Service only needs IFlyable? It gets only IFlyable:",
                "public class FlightSimulator",
                "{",
                "    private IFlyable _subject;",
                "    public FlightSimulator(IFlyable f) => _subject = f;",
                "    public void Simulate() => _subject.Fly();",
                "    // Can't accidentally call Bark() or Swim() — not exposed!",
                "}"
            );

            UI.KeyPoint("Principle of Least Privilege: give each component only the access it actually needs.");
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section5_Repository()
        {
            UI.SubHeader("Repository Pattern — Interfaces in Production");

            UI.Concept(
                "The Repository pattern is the most widely-used interface pattern in enterprise .NET. " +
                "It abstracts data access (database, API, file) behind a collection-like interface. " +
                "Business logic talks to the interface. The interface talks to the actual storage."
            );

            UI.Diagram("Repository Pattern Architecture",
                "  ┌─────────────────────────────────────────────────────────────────┐",
                "  │  Business Layer  (knows only about IProductRepository)          │",
                "  │  OrderService, ProductService, etc.                             │",
                "  └─────────────────────────────────┬───────────────────────────────┘",
                "                                    │ depends on interface",
                "                                    ↓",
                "  ┌─────────────────────────────────────────────────────────────────┐",
                "  │  interface IProductRepository                                   │",
                "  │  { GetById, GetAll, Add, Update, Delete }                      │",
                "  └────┬───────────────────────┬────────────────────────┬───────────┘",
                "       │ implemented by         │                        │",
                "       ↓                        ↓                        ↓",
                "  SqlProductRepo        InMemoryProductRepo      ApiProductRepo",
                "  (prod: SQL Server)    (tests: fast, no DB)    (external API)"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section6_DependencyInjection()
        {
            UI.SubHeader("Dependency Injection — How Interfaces Get Wired Together");

            UI.Concept(
                "Dependency Injection (DI) is how you connect interface-dependent code to " +
                "concrete implementations. Instead of a class creating its own dependencies, " +
                "they're INJECTED from the outside.\n\n" +
                "In ASP.NET Core, you register the mapping and the framework injects automatically."
            );

            UI.Code("Manual DI (understanding before framework magic)",
                "// Your classes depend on interfaces:",
                "public class OrderService",
                "{",
                "    private readonly IProductRepository _products;",
                "    private readonly ILogger            _logger;",
                "",
                "    // Constructor injection: dependencies provided from outside",
                "    public OrderService(IProductRepository products, ILogger logger)",
                "    {",
                "        _products = products;",
                "        _logger   = logger;",
                "    }",
                "}",
                "",
                "// MANUAL WIRING (what DI frameworks automate):",
                "// Production:",
                "var repo    = new SqlProductRepository(connectionString);",
                "var logger  = new FileLogger(\"/logs/app.log\");",
                "var service = new OrderService(repo, logger);",
                "",
                "// Tests — swap implementations with fakes/mocks:",
                "var testRepo    = new InMemoryProductRepository();  // fast, no DB needed",
                "var testLogger  = new ConsoleLogger();",
                "var testService = new OrderService(testRepo, testLogger);"
            );

            UI.Code("ASP.NET Core DI registration (for your real-world projects)",
                "// In Program.cs or Startup.cs:",
                "builder.Services.AddScoped<IProductRepository, SqlProductRepository>();",
                "builder.Services.AddSingleton<ILogger, FileLogger>();",
                "builder.Services.AddTransient<IOrderService, OrderService>();",
                "",
                "// Lifetimes:",
                "// Singleton:  One instance, shared by the entire app lifecycle",
                "// Scoped:     One instance per HTTP request (web apps)",
                "// Transient:  New instance every time it's requested",
                "",
                "// ASP.NET Core automatically injects via constructor:",
                "[ApiController]",
                "public class ProductsController : ControllerBase",
                "{",
                "    private readonly IProductRepository _repo;",
                "    // Framework calls this constructor and injects IProductRepository",
                "    public ProductsController(IProductRepository repo) => _repo = repo;",
                "}"
            );
            UI.Pause();
        }

        // ──────────────────────────────────────────────────────────────
        private static void Section7_LiveDemo()
        {
            UI.SubHeader("Live Demo — Repository Pattern in Action");

            UI.LiveDemo("IProductRepository with InMemory implementation", () =>
            {
                // The service only knows about IProductRepository
                // We inject InMemory — in production this would be SQL
                // InMemoryProductRepository implements both IProductRepository AND IFullRepository.
                // We use IFullRepository here so we can pass it to ProductCatalogService (which needs IReadable).
                IFullRepository repo = new InMemoryProductRepository();
                var catalogService = new ProductCatalogService(repo);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("     FULL CATALOG:");
                Console.ResetColor();
                catalogService.PrintCatalog();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     SEARCH for 'USB':");
                Console.ResetColor();
                foreach (var p in repo.Search("USB"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"     {p}");
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     ADDING new product...");
                Console.ResetColor();
                repo.Add(new Product2 { Name = "USB-C Hub 4-Port", Price = 29.99m, Stock = 15 });
                catalogService.PrintCatalog();

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n     Key: OrderService, ReportService, etc. all use the SAME");
                Console.WriteLine("     IProductRepository. To switch to SQL: one line change at startup.");
                Console.ResetColor();
            });

            UI.Exercise(
                "DESIGN EXERCISE:\n\n" +
                "You're building a weather app. The app needs to:\n" +
                "  1. Get current weather for a city\n" +
                "  2. Get 7-day forecast\n" +
                "  3. Cache results to avoid hitting the API every second\n" +
                "  4. Log all API calls\n\n" +
                "Design the interfaces. What would you segregate?\n" +
                "How would you wire them together?",

                "SUGGESTED INTERFACES:\n\n" +
                "  interface IWeatherProvider\n" +
                "  {\n" +
                "      Task<WeatherData> GetCurrentAsync(string city);\n" +
                "      Task<ForecastData[]> GetForecastAsync(string city, int days);\n" +
                "  }\n\n" +
                "  interface IWeatherCache\n" +
                "  {\n" +
                "      WeatherData Get(string key);\n" +
                "      void Set(string key, WeatherData data, TimeSpan ttl);\n" +
                "  }\n\n" +
                "  IMPLEMENTATIONS:\n" +
                "  OpenWeatherMapProvider : IWeatherProvider  (real API)\n" +
                "  MockWeatherProvider    : IWeatherProvider  (tests)\n" +
                "  MemoryWeatherCache     : IWeatherCache     (production)\n" +
                "  NullWeatherCache       : IWeatherCache     (development)\n\n" +
                "  WeatherService uses IWeatherProvider + IWeatherCache + ILogger\n" +
                "  All injected via constructor — fully testable with mocks."
            );
        }

        // ──────────────────────────────────────────────────────────────
        private static void RunQuiz()
        {
            UI.RunQuiz("Interfaces", new QuizQuestion[]
            {
                new(
                    "What can an interface NOT contain (in traditional C# before version 8)?",
                    new[] {
                        "Method signatures",
                        "Property declarations",
                        "Fields and constructors",
                        "Event declarations"
                    },
                    2,
                    "Interfaces cannot have fields (instance variables) or constructors. They define contracts, not state. C# 8 added default method implementations but fields/constructors are still excluded."
                ),
                new(
                    "A class can inherit from ONE base class. How many interfaces can it implement?",
                    new[] {
                        "One — same restriction as class inheritance",
                        "Two maximum",
                        "As many as needed — no limit",
                        "Three maximum in C#"
                    },
                    2,
                    "C# allows implementing any number of interfaces. This is the mechanism for 'multiple inheritance of contracts' — you can be ILoggable AND ISaveable AND IValidatable."
                ),
                new(
                    "What is the 'Interface Segregation Principle'?",
                    new[] {
                        "Interfaces should be in separate files from their implementations",
                        "All interface methods must be public",
                        "Clients should not be forced to implement methods they don't use — split large interfaces into smaller focused ones",
                        "Interfaces should only have one method"
                    },
                    2,
                    "ISP (the 'I' in SOLID): prefer many small, focused interfaces over one large fat one. Each class implements only the interfaces relevant to its role."
                ),
                new(
                    "Why is constructor injection (passing dependencies via constructor) better than creating them inside the class?",
                    new[] {
                        "Constructors run faster than field initialization",
                        "It enables loose coupling and testability — you can inject test doubles without changing the class",
                        "It reduces memory usage",
                        "C# requires it for classes with interfaces"
                    },
                    1,
                    "Injecting dependencies allows callers to provide different implementations (real vs fake). The class depends on the interface, not a specific implementation."
                ),
                new(
                    "In the Repository pattern, what does the business layer depend on?",
                    new[] {
                        "The concrete SqlRepository class directly",
                        "The specific database technology being used",
                        "The IRepository interface — it never knows or cares about the actual storage",
                        "A static database utility class"
                    },
                    2,
                    "Repository pattern: business logic depends on IRepository. The implementation (SQL, MongoDB, in-memory) is swapped at startup. Business code never changes when storage changes."
                ),
            });
        }
    }
}
