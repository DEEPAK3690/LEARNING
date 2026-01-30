using Microsoft.Extensions.DependencyInjection;

namespace DI_Demo.ServiceLifetimes
{
    // ============================================================
    // SERVICE LIFETIMES IN DETAIL
    // ============================================================
    // Three main lifetimes in .NET DI:
    // 1. TRANSIENT  - New instance every time
    // 2. SCOPED     - One instance per scope (per request in web apps)
    // 3. SINGLETON  - Single instance for entire app lifetime
    // ============================================================

    /// <summary>
    /// Example TRANSIENT service
    /// Use for: Lightweight, stateless services
    /// Behavior: New instance created every time it's requested
    /// </summary>
    public class TransientService
    {
        private readonly Guid _instanceId;
        private readonly DateTime _createdAt;
        private int _callCount = 0;

        public TransientService()
        {
            _instanceId = Guid.NewGuid();
            _createdAt = DateTime.Now;
            Console.WriteLine($"[TransientService] Created new instance: {GetShortId()}");
        }

        public string GetInfo()
        {
            _callCount++;
            return $"Transient #{GetShortId()} - Created at {_createdAt:HH:mm:ss.fff} - Call count: {_callCount}";
        }

        public string GetShortId() => _instanceId.ToString().Substring(0, 8);
    }

    /// <summary>
    /// Example SCOPED service
    /// Use for: Per-request services (database contexts, request-specific data)
    /// Behavior: One instance per scope (in web apps, typically per HTTP request)
    /// </summary>
    public class ScopedService
    {
        private readonly Guid _instanceId;
        private readonly DateTime _createdAt;
        private int _callCount = 0;

        public ScopedService()
        {
            _instanceId = Guid.NewGuid();
            _createdAt = DateTime.Now;
            Console.WriteLine($"[ScopedService] Created new instance: {GetShortId()}");
        }

        public string GetInfo()
        {
            _callCount++;
            return $"Scoped #{GetShortId()} - Created at {_createdAt:HH:mm:ss.fff} - Call count: {_callCount}";
        }

        public string GetShortId() => _instanceId.ToString().Substring(0, 8);
    }

    /// <summary>
    /// Example SINGLETON service
    /// Use for: Expensive to create, shared state, configuration
    /// Behavior: Single instance shared across entire application
    /// </summary>
    public class SingletonService
    {
        private readonly Guid _instanceId;
        private readonly DateTime _createdAt;
        private int _callCount = 0;

        public SingletonService()
        {
            _instanceId = Guid.NewGuid();
            _createdAt = DateTime.Now;
            Console.WriteLine($"[SingletonService] Created new instance: {GetShortId()}");
        }

        public string GetInfo()
        {
            _callCount++;
            return $"Singleton #{GetShortId()} - Created at {_createdAt:HH:mm:ss.fff} - Call count: {_callCount}";
        }

        public string GetShortId() => _instanceId.ToString().Substring(0, 8);
    }

    // ============================================================
    // REAL-WORLD SERVICE EXAMPLES
    // ============================================================

    /// <summary>
    /// SINGLETON Example: Application Configuration
    /// Thread-safe, expensive to load, shared across app
    /// </summary>
    public class AppConfiguration
    {
        private readonly Dictionary<string, string> _settings;
        private readonly Guid _instanceId;

        public AppConfiguration()
        {
            _instanceId = Guid.NewGuid();
            Console.WriteLine($"[AppConfiguration SINGLETON] Loading configuration... ID: {GetShortId()}");
            
            // Simulate expensive operation (loading from file/database)
            Thread.Sleep(100);
            
            _settings = new Dictionary<string, string>
            {
                { "AppName", "DI Demo Application" },
                { "Version", "1.0.0" },
                { "Environment", "Development" }
            };
        }

        public string GetSetting(string key)
        {
            return _settings.ContainsKey(key) 
                ? $"{_settings[key]} (from instance {GetShortId()})" 
                : "Not found";
        }

        public string GetShortId() => _instanceId.ToString().Substring(0, 8);
    }

    /// <summary>
    /// SINGLETON Example: Cache Service
    /// Shared cache across entire application
    /// </summary>
    public class CacheService
    {
        private readonly Dictionary<string, object> _cache = new();
        private readonly Guid _instanceId;

        public CacheService()
        {
            _instanceId = Guid.NewGuid();
            Console.WriteLine($"[CacheService SINGLETON] Initialized cache... ID: {GetShortId()}");
        }

        public void Set(string key, object value)
        {
            _cache[key] = value;
            Console.WriteLine($"[Cache {GetShortId()}] Cached: {key}");
        }

        public T? Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                Console.WriteLine($"[Cache {GetShortId()}] Retrieved: {key}");
                return (T)value;
            }
            Console.WriteLine($"[Cache {GetShortId()}] Miss: {key}");
            return default;
        }

        public int GetItemCount() => _cache.Count;
        public string GetShortId() => _instanceId.ToString().Substring(0, 8);
    }

    /// <summary>
    /// SCOPED Example: Database Context (simulated)
    /// One instance per request, manages transaction scope
    /// </summary>
    public class DatabaseContext
    {
        private readonly Guid _instanceId;
        private readonly List<string> _operations = new();

        public DatabaseContext()
        {
            _instanceId = Guid.NewGuid();
            Console.WriteLine($"[DatabaseContext SCOPED] Opened connection... ID: {GetShortId()}");
        }

        public void ExecuteQuery(string query)
        {
            _operations.Add(query);
            Console.WriteLine($"[DB {GetShortId()}] Executed: {query}");
        }

        public void SaveChanges()
        {
            Console.WriteLine($"[DB {GetShortId()}] Saved {_operations.Count} operations");
        }

        public string GetShortId() => _instanceId.ToString().Substring(0, 8);
    }

    /// <summary>
    /// SCOPED Example: Request Context
    /// Stores request-specific data (user, correlation ID, etc.)
    /// </summary>
    public class RequestContext
    {
        private readonly Guid _requestId;
        private readonly DateTime _requestTime;
        public string? UserName { get; set; }

        public RequestContext()
        {
            _requestId = Guid.NewGuid();
            _requestTime = DateTime.Now;
            Console.WriteLine($"[RequestContext SCOPED] New request: {GetShortId()}");
        }

        public string GetRequestInfo()
        {
            return $"Request {GetShortId()} by {UserName ?? "Anonymous"} at {_requestTime:HH:mm:ss.fff}";
        }

        public string GetShortId() => _requestId.ToString().Substring(0, 8);
    }

    /// <summary>
    /// TRANSIENT Example: Email Validator
    /// Lightweight, stateless, no reason to reuse
    /// </summary>
    public class EmailValidator
    {
        private readonly Guid _instanceId;

        public EmailValidator()
        {
            _instanceId = Guid.NewGuid();
            Console.WriteLine($"[EmailValidator TRANSIENT] Created: {GetShortId()}");
        }

        public bool IsValid(string email)
        {
            Console.WriteLine($"[EmailValidator {GetShortId()}] Validating: {email}");
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }

        public string GetShortId() => _instanceId.ToString().Substring(0, 8);
    }

    /// <summary>
    /// TRANSIENT Example: Price Calculator
    /// Stateless calculation service
    /// </summary>
    public class PriceCalculator
    {
        private readonly Guid _instanceId;

        public PriceCalculator()
        {
            _instanceId = Guid.NewGuid();
            Console.WriteLine($"[PriceCalculator TRANSIENT] Created: {GetShortId()}");
        }

        public decimal CalculateTax(decimal amount, decimal taxRate)
        {
            Console.WriteLine($"[PriceCalculator {GetShortId()}] Calculating tax on ${amount}");
            return amount * taxRate;
        }

        public string GetShortId() => _instanceId.ToString().Substring(0, 8);
    }

    // ============================================================
    // SERVICE THAT USES MULTIPLE DEPENDENCIES WITH DIFFERENT LIFETIMES
    // ============================================================

    /// <summary>
    /// Example service showing how to use dependencies with different lifetimes
    /// </summary>
    public class OrderProcessor
    {
        private readonly TransientService _transient;
        private readonly ScopedService _scoped;
        private readonly SingletonService _singleton;
        private readonly AppConfiguration _config;
        private readonly CacheService _cache;

        public OrderProcessor(
            TransientService transient,
            ScopedService scoped,
            SingletonService singleton,
            AppConfiguration config,
            CacheService cache)
        {
            _transient = transient;
            _scoped = scoped;
            _singleton = singleton;
            _config = config;
            _cache = cache;
            
            Console.WriteLine($"[OrderProcessor] Created with:");
            Console.WriteLine($"  - Transient: {_transient.GetShortId()}");
            Console.WriteLine($"  - Scoped: {_scoped.GetShortId()}");
            Console.WriteLine($"  - Singleton: {_singleton.GetShortId()}");
        }

        public void ProcessOrder(string orderId)
        {
            Console.WriteLine($"\n[OrderProcessor] Processing order: {orderId}");
            Console.WriteLine($"  {_transient.GetInfo()}");
            Console.WriteLine($"  {_scoped.GetInfo()}");
            Console.WriteLine($"  {_singleton.GetInfo()}");
            Console.WriteLine($"  App: {_config.GetSetting("AppName")}");
            
            _cache.Set($"order_{orderId}", $"Processed at {DateTime.Now}");
        }
    }

    /// <summary>
    /// Another service using the same dependencies
    /// </summary>
    public class ShippingProcessor
    {
        private readonly TransientService _transient;
        private readonly ScopedService _scoped;
        private readonly SingletonService _singleton;

        public ShippingProcessor(
            TransientService transient,
            ScopedService scoped,
            SingletonService singleton)
        {
            _transient = transient;
            _scoped = scoped;
            _singleton = singleton;
            
            Console.WriteLine($"[ShippingProcessor] Created with:");
            Console.WriteLine($"  - Transient: {_transient.GetShortId()}");
            Console.WriteLine($"  - Scoped: {_scoped.GetShortId()}");
            Console.WriteLine($"  - Singleton: {_singleton.GetShortId()}");
        }

        public void ProcessShipping(string orderId)
        {
            Console.WriteLine($"\n[ShippingProcessor] Processing shipping for: {orderId}");
            Console.WriteLine($"  {_transient.GetInfo()}");
            Console.WriteLine($"  {_scoped.GetInfo()}");
            Console.WriteLine($"  {_singleton.GetInfo()}");
        }
    }

    // ============================================================
    // LIFETIME COMPARISON HELPER
    // ============================================================

    /// <summary>
    /// Helper class to demonstrate lifetime differences
    /// </summary>
    public class LifetimeDemo
    {
        public static void ShowLifetimeBehavior(IServiceProvider serviceProvider, string scopeName)
        {
            Console.WriteLine($"\n{'='.ToString().PadRight(60, '=')}");
            Console.WriteLine($"SCOPE: {scopeName}");
            Console.WriteLine($"{'='.ToString().PadRight(60, '=')}");

            // Request services multiple times
            Console.WriteLine("\n--- First Request ---");
            var transient1 = serviceProvider.GetRequiredService<TransientService>();
            var scoped1 = serviceProvider.GetRequiredService<ScopedService>();
            var singleton1 = serviceProvider.GetRequiredService<SingletonService>();

            Console.WriteLine($"Transient 1: {transient1.GetShortId()}");
            Console.WriteLine($"Scoped 1:    {scoped1.GetShortId()}");
            Console.WriteLine($"Singleton 1: {singleton1.GetShortId()}");

            Console.WriteLine("\n--- Second Request (same scope) ---");
            var transient2 = serviceProvider.GetRequiredService<TransientService>();
            var scoped2 = serviceProvider.GetRequiredService<ScopedService>();
            var singleton2 = serviceProvider.GetRequiredService<SingletonService>();

            Console.WriteLine($"Transient 2: {transient2.GetShortId()}");
            Console.WriteLine($"Scoped 2:    {scoped2.GetShortId()}");
            Console.WriteLine($"Singleton 2: {singleton2.GetShortId()}");

            Console.WriteLine("\n--- Comparison ---");
            Console.WriteLine($"Transient: {(transient1.GetShortId() == transient2.GetShortId() ? "SAME ❌ (should be different)" : "DIFFERENT ✅")}");
            Console.WriteLine($"Scoped:    {(scoped1.GetShortId() == scoped2.GetShortId() ? "SAME ✅ (same scope)" : "DIFFERENT ❌")}");
            Console.WriteLine($"Singleton: {(singleton1.GetShortId() == singleton2.GetShortId() ? "SAME ✅ (always same)" : "DIFFERENT ❌")}");
        }
    }
}
