namespace DI_Demo.WithDI
{
    // ============================================================
    // WITH DEPENDENCY INJECTION - LOOSE COUPLING
    // ============================================================
    // Benefits:
    // 1. Loose coupling - Services depend on abstractions (interfaces)
    // 2. Easy to test - Can inject mock implementations
    // 3. Easy to change - Can swap implementations without modifying code
    // 4. Follows Dependency Inversion Principle - Depend on abstractions
    // 5. Flexible - Can change implementations at runtime
    // 6. Better separation of concerns
    // ============================================================

    /// <summary>
    /// Interface for notification service - ABSTRACTION
    /// This is the key to loose coupling
    /// </summary>
    public interface INotificationService
    {
        void SendNotification(string recipient, string message);
    }

    /// <summary>
    /// Email implementation of notification service
    /// </summary>
    public class EmailNotificationService : INotificationService
    {
        public void SendNotification(string recipient, string message)
        {
            Console.WriteLine($"[EmailNotificationService] Sending email to {recipient}: {message}");
            // Real implementation would send email via SMTP
        }
    }

    /// <summary>
    /// SMS implementation of notification service
    /// </summary>
    public class SmsNotificationService : INotificationService
    {
        public void SendNotification(string recipient, string message)
        {
            Console.WriteLine($"[SmsNotificationService] Sending SMS to {recipient}: {message}");
            // Real implementation would send SMS via API
        }
    }

    /// <summary>
    /// Push notification implementation
    /// </summary>
    public class PushNotificationService : INotificationService
    {
        public void SendNotification(string recipient, string message)
        {
            Console.WriteLine($"[PushNotificationService] Sending push notification to {recipient}: {message}");
            // Real implementation would send push notification
        }
    }

    /// <summary>
    /// User service WITH dependency injection
    /// BENEFIT: Depends on INotificationService interface, not concrete implementation
    /// </summary>
    public class UserService
    {
        // Depend on abstraction (interface), not concrete class
        private readonly INotificationService _notificationService;

        // Constructor Injection - Most common DI pattern
        // The dependency is injected through the constructor
        public UserService(INotificationService notificationService)
        {
            // BENEFIT: We receive the dependency from outside
            // We don't create it ourselves
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public void RegisterUser(string username, string recipient)
        {
            Console.WriteLine($"[UserService] Registering user: {username}");
            
            // Simulate user registration logic
            Console.WriteLine($"[UserService] Saving user to database...");
            
            // Send notification using injected service
            // We don't care if it's email, SMS, or push notification!
            _notificationService.SendNotification(recipient, $"Welcome {username}!");
            
            Console.WriteLine($"[UserService] User {username} registered successfully!\n");
        }
    }

    /// <summary>
    /// Order service WITH dependency injection
    /// </summary>
    public class OrderService
    {
        private readonly INotificationService _notificationService;

        // Constructor Injection
        public OrderService(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public void PlaceOrder(string orderId, string customerContact)
        {
            Console.WriteLine($"[OrderService] Placing order: {orderId}");
            
            // Process order (simulated)
            Console.WriteLine($"[OrderService] Processing payment...");
            
            // Send confirmation using injected service
            _notificationService.SendNotification(customerContact, $"Order {orderId} confirmed!");
            
            Console.WriteLine($"[OrderService] Order {orderId} placed successfully!\n");
        }
    }

    /// <summary>
    /// Example of a service with multiple dependencies
    /// Shows how to inject multiple services
    /// </summary>
    public interface ILoggerService
    {
        void Log(string message);
    }

    public class ConsoleLoggerService : ILoggerService
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {DateTime.Now}: {message}");
        }
    }

    /// <summary>
    /// Payment service with multiple dependencies
    /// </summary>
    public class PaymentService
    {
        private readonly INotificationService _notificationService;
        private readonly ILoggerService _loggerService;

        // Multiple dependencies injected through constructor
        public PaymentService(INotificationService notificationService, ILoggerService loggerService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));
        }

        public void ProcessPayment(string paymentId, string customerContact, decimal amount)
        {
            _loggerService.Log($"Processing payment {paymentId} for amount ${amount}");
            
            Console.WriteLine($"[PaymentService] Processing payment {paymentId}...");
            
            // Process payment logic (simulated)
            Thread.Sleep(100); // Simulate processing time
            
            _loggerService.Log($"Payment {paymentId} processed successfully");
            
            // Send notification
            _notificationService.SendNotification(customerContact, 
                $"Payment of ${amount} processed successfully!");
            
            Console.WriteLine($"[PaymentService] Payment {paymentId} completed!\n");
        }
    }

    // ============================================================
    // DIFFERENT DI LIFETIMES
    // ============================================================
    // 1. Transient: New instance every time it's requested
    // 2. Scoped: One instance per scope (e.g., per HTTP request)
    // 3. Singleton: Single instance for the entire application lifetime
    // ============================================================

    /// <summary>
    /// Example service to demonstrate Singleton lifetime
    /// </summary>
    public class ConfigurationService
    {
        private readonly Guid _instanceId;

        public ConfigurationService()
        {
            _instanceId = Guid.NewGuid();
            Console.WriteLine($"[ConfigurationService] Created with ID: {_instanceId}");
        }

        public string GetConfig(string key)
        {
            return $"Config[{key}] from instance {_instanceId.ToString().Substring(0, 8)}";
        }
    }

    /// <summary>
    /// Example service to demonstrate Transient lifetime
    /// </summary>
    public class TemporaryService
    {
        private readonly Guid _instanceId;

        public TemporaryService()
        {
            _instanceId = Guid.NewGuid();
            Console.WriteLine($"[TemporaryService] Created with ID: {_instanceId}");
        }

        public string GetId()
        {
            return _instanceId.ToString().Substring(0, 8);
        }
    }
}
