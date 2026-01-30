using Microsoft.Extensions.DependencyInjection;
using DI_Demo.WithoutDI;
using DI_Demo.WithDI;
using DI_Demo.ServiceLifetimes;

namespace DI_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("DEPENDENCY INJECTION DEMONSTRATION IN .NET");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine();

            // ============================================================
            // PART 1: WITHOUT DEPENDENCY INJECTION
            // ============================================================
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  PART 1: WITHOUT DEPENDENCY INJECTION (Tight Coupling)      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            DemoWithoutDI();

            // Wait for user input
            Console.WriteLine("\nPress any key to continue to Part 2...");
            Console.ReadKey();
            Console.Clear();

            // ============================================================
            // PART 2: WITH DEPENDENCY INJECTION
            // ============================================================
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  PART 2: WITH DEPENDENCY INJECTION (Loose Coupling)          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            DemoWithDI();

            // ============================================================
            // PART 3: DI SERVICE LIFETIMES - BASIC
            // ============================================================
            Console.WriteLine("\n\nPress any key to continue to Part 3...");
            Console.ReadKey();
            Console.Clear();

            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  PART 3: SERVICE LIFETIMES - Basic Demonstration            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            DemoServiceLifetimesBasic();

            // ============================================================
            // PART 4: DI SERVICE LIFETIMES - ADVANCED
            // ============================================================
            Console.WriteLine("\n\nPress any key to continue to Part 4...");
            Console.ReadKey();
            Console.Clear();

            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  PART 4: SERVICE LIFETIMES - Real-World Examples            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            DemoServiceLifetimesAdvanced();

            // ============================================================
            // PART 5: SERVICE LIFETIMES WITH SCOPES
            // ============================================================
            Console.WriteLine("\n\nPress any key to continue to Part 5...");
            Console.ReadKey();
            Console.Clear();

            Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  PART 5: SERVICE LIFETIMES - Scopes (Simulating Requests)   ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            DemoServiceLifetimesWithScopes();

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Demonstrates the problems with tight coupling (WITHOUT DI)
        /// </summary>
        static void DemoWithoutDI()
        {
            Console.WriteLine("Problems with this approach:");
            Console.WriteLine("❌ Services create their own dependencies (EmailService)");
            Console.WriteLine("❌ Hard to test - cannot mock EmailService");
            Console.WriteLine("❌ Hard to change - need to modify class to use SMS instead");
            Console.WriteLine("❌ Tight coupling - classes depend on concrete implementations\n");

            // Create services - they will create their own dependencies internally
            var userService = new WithoutDI.UserService();
            var orderService = new WithoutDI.OrderService();

            // Use the services
            userService.RegisterUser("JohnDoe", "john@example.com");
            orderService.PlaceOrder("ORD-12345", "john@example.com");

            Console.WriteLine("\n💡 What if we want to use SMS instead of Email?");
            Console.WriteLine("   We would need to MODIFY the UserService and OrderService classes!");
            Console.WriteLine("   This violates the Open/Closed Principle.");
        }

        /// <summary>
        /// Demonstrates the benefits of loose coupling (WITH DI)
        /// </summary>
        static void DemoWithDI()
        {
            Console.WriteLine("Benefits of this approach:");
            Console.WriteLine("✅ Services receive dependencies through constructor");
            Console.WriteLine("✅ Easy to test - can inject mock implementations");
            Console.WriteLine("✅ Easy to change - just register different implementation");
            Console.WriteLine("✅ Loose coupling - classes depend on interfaces\n");

            // ============================================================
            // Configure Dependency Injection Container
            // ============================================================
            var services = new ServiceCollection();

            // Register services with their interfaces
            // We can easily swap implementations here!
            
            Console.WriteLine("--- Scenario 1: Using Email Notifications ---\n");
            services.AddTransient<INotificationService, EmailNotificationService>();
            services.AddTransient<ILoggerService, ConsoleLoggerService>();
            services.AddTransient<WithDI.UserService>();
            services.AddTransient<WithDI.OrderService>();
            services.AddTransient<WithDI.PaymentService>();

            // Build the service provider (DI container)
            var serviceProvider = services.BuildServiceProvider();

            // Get services from the container - it will automatically inject dependencies
            var userService = serviceProvider.GetRequiredService<WithDI.UserService>();
            var orderService = serviceProvider.GetRequiredService<WithDI.OrderService>();
            var paymentService = serviceProvider.GetRequiredService<WithDI.PaymentService>();

            // Use the services
            userService.RegisterUser("JohnDoe", "john@example.com");
            orderService.PlaceOrder("ORD-12345", "john@example.com");
            paymentService.ProcessPayment("PAY-789", "john@example.com", 99.99m);

            Console.WriteLine("\n--- Scenario 2: Switching to SMS Notifications ---");
            Console.WriteLine("💡 We just change ONE LINE in the DI configuration!\n");

            // Create a new container with SMS service
            var services2 = new ServiceCollection();
            
            // ONLY THIS LINE CHANGED! Everything else remains the same
            services2.AddTransient<INotificationService, SmsNotificationService>();
            services2.AddTransient<ILoggerService, ConsoleLoggerService>();
            services2.AddTransient<WithDI.UserService>();
            services2.AddTransient<WithDI.OrderService>();
            services2.AddTransient<WithDI.PaymentService>();

            var serviceProvider2 = services2.BuildServiceProvider();
            var userService2 = serviceProvider2.GetRequiredService<WithDI.UserService>();
            var orderService2 = serviceProvider2.GetRequiredService<WithDI.OrderService>();

            userService2.RegisterUser("JaneDoe", "+1-555-0123");
            orderService2.PlaceOrder("ORD-67890", "+1-555-0123");

            Console.WriteLine("\n--- Scenario 3: Using Push Notifications ---\n");

            var services3 = new ServiceCollection();
            services3.AddTransient<INotificationService, PushNotificationService>();
            services3.AddTransient<WithDI.UserService>();

            var serviceProvider3 = services3.BuildServiceProvider();
            var userService3 = serviceProvider3.GetRequiredService<WithDI.UserService>();

            userService3.RegisterUser("BobSmith", "device-token-abc123");

            Console.WriteLine("\n✅ Notice: We didn't modify UserService, OrderService, or PaymentService!");
            Console.WriteLine("   We just changed the DI configuration!");
        }

        /// <summary>
        /// Demonstrates basic service lifetimes
        /// </summary>
        static void DemoServiceLifetimesBasic()
        {
            Console.WriteLine("📖 SERVICE LIFETIMES EXPLAINED:\n");
            Console.WriteLine("1. TRANSIENT  - New instance EVERY time requested");
            Console.WriteLine("2. SCOPED     - One instance PER SCOPE (e.g., per web request)");
            Console.WriteLine("3. SINGLETON  - ONE instance for ENTIRE application lifetime\n");

            var services = new ServiceCollection();

            // Register all three lifetime types
            services.AddTransient<TransientService>();
            services.AddScoped<ScopedService>();
            services.AddSingleton<SingletonService>();

            var serviceProvider = services.BuildServiceProvider();

            // Demonstrate in same scope
            LifetimeDemo.ShowLifetimeBehavior(serviceProvider, "Main Application Scope");

            Console.WriteLine("\n\n💡 KEY OBSERVATIONS:");
            Console.WriteLine("   Transient: New instance each time (different IDs)");
            Console.WriteLine("   Scoped:    Same instance within scope (same IDs)");
            Console.WriteLine("   Singleton: Always same instance (same IDs everywhere)");
        }

        /// <summary>
        /// Demonstrates real-world service lifetime usage
        /// </summary>
        static void DemoServiceLifetimesAdvanced()
        {
            Console.WriteLine("🏭 REAL-WORLD SERVICE EXAMPLES:\n");

            var services = new ServiceCollection();

            // SINGLETON examples - expensive to create, shared state
            services.AddSingleton<AppConfiguration>();
            services.AddSingleton<CacheService>();

            // SCOPED examples - per-request data
            services.AddScoped<DatabaseContext>();
            services.AddScoped<RequestContext>();

            // TRANSIENT examples - lightweight, stateless
            services.AddTransient<EmailValidator>();
            services.AddTransient<PriceCalculator>();

            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("--- Singleton Services (Expensive, Shared) ---\n");
            
            var config1 = serviceProvider.GetRequiredService<AppConfiguration>();
            Console.WriteLine($"Config: {config1.GetSetting("AppName")}");
            
            var config2 = serviceProvider.GetRequiredService<AppConfiguration>();
            Console.WriteLine($"Config: {config2.GetSetting("Version")}");
            Console.WriteLine($"✅ Both requests used SAME instance: {config1.GetShortId() == config2.GetShortId()}\n");

            var cache = serviceProvider.GetRequiredService<CacheService>();
            cache.Set("user_123", "John Doe");
            cache.Set("user_456", "Jane Smith");
            
            var cache2 = serviceProvider.GetRequiredService<CacheService>();
            var userName = cache2.Get<string>("user_123");
            Console.WriteLine($"✅ Cache shared across requests: Found '{userName}'");
            Console.WriteLine($"   Cache has {cache2.GetItemCount()} items\n");

            Console.WriteLine("\n--- Transient Services (Lightweight, Stateless) ---\n");
            
            var validator1 = serviceProvider.GetRequiredService<EmailValidator>();
            Console.WriteLine($"Valid: {validator1.IsValid("test@example.com")}");
            
            var validator2 = serviceProvider.GetRequiredService<EmailValidator>();
            Console.WriteLine($"Valid: {validator2.IsValid("invalid-email")}");
            Console.WriteLine($"✅ Each request gets NEW instance: {validator1.GetShortId() != validator2.GetShortId()}\n");

            var calc1 = serviceProvider.GetRequiredService<PriceCalculator>();
            var tax = calc1.CalculateTax(100m, 0.08m);
            Console.WriteLine($"Tax calculated: ${tax}");

            Console.WriteLine("\n\n💡 WHEN TO USE EACH LIFETIME:");
            Console.WriteLine("\n📌 SINGLETON:");
            Console.WriteLine("   ✅ Configuration (loaded once)");
            Console.WriteLine("   ✅ Caching (shared across app)");
            Console.WriteLine("   ✅ Logging (single logger instance)");
            Console.WriteLine("   ⚠️  Must be thread-safe!");
            
            Console.WriteLine("\n📌 SCOPED:");
            Console.WriteLine("   ✅ Database contexts (per request)");
            Console.WriteLine("   ✅ Request-specific data (user, correlation ID)");
            Console.WriteLine("   ✅ Unit of Work pattern");
            Console.WriteLine("   ⚠️  Mainly for web applications!");
            
            Console.WriteLine("\n📌 TRANSIENT:");
            Console.WriteLine("   ✅ Lightweight services");
            Console.WriteLine("   ✅ Stateless operations");
            Console.WriteLine("   ✅ Validators, calculators, helpers");
            Console.WriteLine("   ⚠️  Can create many instances - keep lightweight!");
        }

        /// <summary>
        /// Demonstrates scoped services with multiple scopes (simulating web requests)
        /// </summary>
        static void DemoServiceLifetimesWithScopes()
        {
            Console.WriteLine("🌐 SIMULATING WEB REQUESTS WITH SCOPES:\n");
            Console.WriteLine("In web apps, each HTTP request creates a new scope.");
            Console.WriteLine("Scoped services live for the duration of that request.\n");

            var services = new ServiceCollection();

            // Register services with different lifetimes
            services.AddTransient<TransientService>();
            services.AddScoped<ScopedService>();
            services.AddSingleton<SingletonService>();
            services.AddSingleton<AppConfiguration>();
            services.AddSingleton<CacheService>();
            
            // Register processors that use these services
            services.AddScoped<OrderProcessor>();
            services.AddScoped<ShippingProcessor>();

            var serviceProvider = services.BuildServiceProvider();

            // Simulate Request 1
            Console.WriteLine("\n" + "=".PadRight(70, '='));
            Console.WriteLine("🌐 SIMULATING REQUEST 1 (New Scope)");
            Console.WriteLine("=".PadRight(70, '='));
            
            using (var scope1 = serviceProvider.CreateScope())
            {
                Console.WriteLine("\n--- Processing Order in Request 1 ---");
                var orderProcessor1 = scope1.ServiceProvider.GetRequiredService<OrderProcessor>();
                orderProcessor1.ProcessOrder("ORD-001");

                Console.WriteLine("\n--- Processing Shipping in Request 1 (Same Scope) ---");
                var shippingProcessor1 = scope1.ServiceProvider.GetRequiredService<ShippingProcessor>();
                shippingProcessor1.ProcessShipping("ORD-001");

                Console.WriteLine("\n💡 OBSERVATIONS in Request 1:");
                Console.WriteLine("   - Transient: Different for OrderProcessor and ShippingProcessor");
                Console.WriteLine("   - Scoped:    SAME for OrderProcessor and ShippingProcessor");
                Console.WriteLine("   - Singleton: SAME everywhere (as always)");
            }

            // Simulate Request 2
            Console.WriteLine("\n\n" + "=".PadRight(70, '='));
            Console.WriteLine("🌐 SIMULATING REQUEST 2 (New Scope)");
            Console.WriteLine("=".PadRight(70, '='));
            
            using (var scope2 = serviceProvider.CreateScope())
            {
                Console.WriteLine("\n--- Processing Order in Request 2 ---");
                var orderProcessor2 = scope2.ServiceProvider.GetRequiredService<OrderProcessor>();
                orderProcessor2.ProcessOrder("ORD-002");

                Console.WriteLine("\n--- Processing Shipping in Request 2 (Same Scope) ---");
                var shippingProcessor2 = scope2.ServiceProvider.GetRequiredService<ShippingProcessor>();
                shippingProcessor2.ProcessShipping("ORD-002");

                Console.WriteLine("\n💡 OBSERVATIONS in Request 2:");
                Console.WriteLine("   - Transient: Different instances (as always)");
                Console.WriteLine("   - Scoped:    NEW instances (different scope from Request 1)");
                Console.WriteLine("   - Singleton: SAME instance (shared across all requests)");
            }

            // Simulate Request 3
            Console.WriteLine("\n\n" + "=".PadRight(70, '='));
            Console.WriteLine("🌐 SIMULATING REQUEST 3 (New Scope)");
            Console.WriteLine("=".PadRight(70, '='));
            
            using (var scope3 = serviceProvider.CreateScope())
            {
                var orderProcessor3 = scope3.ServiceProvider.GetRequiredService<OrderProcessor>();
                orderProcessor3.ProcessOrder("ORD-003");
            }

            Console.WriteLine("\n\n" + "=".PadRight(70, '='));
            Console.WriteLine("📊 SUMMARY: Service Lifetime Behavior");
            Console.WriteLine("=".PadRight(70, '='));
            
            Console.WriteLine("\n┌─────────────┬──────────────────┬────────────────────┬─────────────┐");
            Console.WriteLine("│ Lifetime    │ Within Request   │ Across Requests    │ Use Case    │");
            Console.WriteLine("├─────────────┼──────────────────┼────────────────────┼─────────────┤");
            Console.WriteLine("│ Transient   │ Always different │ Always different   │ Lightweight │");
            Console.WriteLine("│ Scoped      │ Same instance    │ Different instance │ Per-request │");
            Console.WriteLine("│ Singleton   │ Same instance    │ Same instance      │ App-wide    │");
            Console.WriteLine("└─────────────┴──────────────────┴────────────────────┴─────────────┘");

            Console.WriteLine("\n🎯 REAL-WORLD ANALOGY:");
            Console.WriteLine("\n   Restaurant scenario:");
            Console.WriteLine("   🍽️  Transient  = Paper napkin (new one each time)");
            Console.WriteLine("   👤 Scoped     = Your waiter (same for your meal, different for other tables)");
            Console.WriteLine("   🏛️  Singleton  = The restaurant building (same for everyone)");

            Console.WriteLine("\n⚠️  IMPORTANT RULES:");
            Console.WriteLine("   1. Singleton can't depend on Scoped or Transient (lifecycle violation)");
            Console.WriteLine("   2. Scoped can depend on Singleton and Transient ✅");
            Console.WriteLine("   3. Transient can depend on Singleton and Scoped ✅");
            Console.WriteLine("   4. Scoped services are disposed at end of scope");
            Console.WriteLine("   5. Singleton services live until app shutdown");
        }
    }
}
