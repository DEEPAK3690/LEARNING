namespace MyWebApplication.Models.DIExamples
{
    // ============================================================
    // PRACTICAL EXAMPLES OF DI LIFETIMES IN REAL SCENARIOS
    // ============================================================

    // ============ EXAMPLE 1: DATABASE CONTEXT (SCOPED) ============
    public interface IAppDbContext
    {
        void SaveChanges();
    }

    public class AppDbContext : IAppDbContext
    {
        private readonly string _connectionId;

        public AppDbContext()
        {
            _connectionId = Guid.NewGuid().ToString();
            Console.WriteLine($"[DbContext] Opened connection: {_connectionId}");
        }

        public void SaveChanges()
        {
            Console.WriteLine($"[DbContext] Saving changes to connection: {_connectionId}");
        }

        ~AppDbContext()
        {
            Console.WriteLine($"[DbContext] Closed connection: {_connectionId}");
        }
    }

    // Registration (in Program.cs):
    // builder.Services.AddScoped<IAppDbContext, AppDbContext>();


    // ============ EXAMPLE 2: CONFIGURATION (SINGLETON) ============
    public interface IAppConfig
    {
        string GetConnectionString();
        string GetApiKey();
    }

    public class AppConfig : IAppConfig
    {
        private readonly string _connectionString;
        private readonly string _apiKey;

        public AppConfig()
        {
            _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? "DefaultConnection";
            _apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "DefaultKey";
            Console.WriteLine("[AppConfig] Configuration loaded (once for entire app)");
        }

        public string GetConnectionString() => _connectionString;
        public string GetApiKey() => _apiKey;
    }

    // Registration (in Program.cs):
    // builder.Services.AddSingleton<IAppConfig, AppConfig>();


    // ============ EXAMPLE 3: LOGGING SERVICE (TRANSIENT) ============
    public interface ILogService
    {
        void Log(string message);
    }

    public class LogService : ILogService
    {
        private readonly string _instanceId;

        public LogService()
        {
            _instanceId = Guid.NewGuid().ToString().Substring(0, 8);
            Console.WriteLine($"[LogService] Instance created: {_instanceId}");
        }

        public void Log(string message)
        {
            Console.WriteLine($"[{_instanceId}] {message}");
        }
    }

    // Registration (in Program.cs):
    // builder.Services.AddTransient<ILogService, LogService>();


    // ============ EXAMPLE 4: REQUEST CONTEXT (SCOPED) ============
    public interface IRequestContext
    {
        string GetRequestId();
        DateTime GetStartTime();
    }

    public class RequestContext : IRequestContext
    {
        private readonly string _requestId;
        private readonly DateTime _startTime;

        public RequestContext()
        {
            _requestId = Guid.NewGuid().ToString();
            _startTime = DateTime.UtcNow;
            Console.WriteLine($"[RequestContext] Request {_requestId} started");
        }

        public string GetRequestId() => _requestId;
        public DateTime GetStartTime() => _startTime;
    }

    // Registration (in Program.cs):
    // builder.Services.AddScoped<IRequestContext, RequestContext>();


    // ============ EXAMPLE 5: MEMORY CACHE (SINGLETON) ============
    public interface IMemoryCache
    {
        void Set<T>(string key, T value);
        T? Get<T>(string key);
    }

    public class MemoryCache : IMemoryCache
    {
        private readonly Dictionary<string, object> _cache = new();
        private readonly object _lockObject = new();

        public void Set<T>(string key, T value)
        {
            lock (_lockObject)
            {
                _cache[key] = value!;
                Console.WriteLine($"[MemoryCache] Set key: {key}");
            }
        }

        public T? Get<T>(string key)
        {
            lock (_lockObject)
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    Console.WriteLine($"[MemoryCache] Get key: {key} (HIT)");
                    return (T?)value;
                }
                Console.WriteLine($"[MemoryCache] Get key: {key} (MISS)");
                return default;
            }
        }
    }

    // Registration (in Program.cs):
    // builder.Services.AddSingleton<IMemoryCache, MemoryCache>();


    // ============ EXAMPLE 6: DATA VALIDATOR (TRANSIENT) ============
    public interface IValidator
    {
        bool IsValidEmail(string email);
        bool IsValidPhone(string phone);
    }

    public class Validator : IValidator
    {
        private readonly string _instanceId = Guid.NewGuid().ToString().Substring(0, 8);

        public bool IsValidEmail(string email)
        {
            var isValid = email.Contains("@");
            Console.WriteLine($"[Validator-{_instanceId}] Email validation: {email} = {isValid}");
            return isValid;
        }

        public bool IsValidPhone(string phone)
        {
            var isValid = phone.Length >= 10;
            Console.WriteLine($"[Validator-{_instanceId}] Phone validation: {phone} = {isValid}");
            return isValid;
        }
    }

    // Registration (in Program.cs):
    // builder.Services.AddTransient<IValidator, Validator>();
}
