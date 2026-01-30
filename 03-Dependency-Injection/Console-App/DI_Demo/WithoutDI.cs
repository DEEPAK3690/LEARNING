namespace DI_Demo.WithoutDI
{
    // ============================================================
    // WITHOUT DEPENDENCY INJECTION - TIGHT COUPLING
    // ============================================================
    // Problems:
    // 1. Tight coupling - UserService directly creates EmailService
    // 2. Hard to test - Cannot mock EmailService
    // 3. Hard to change - If we want to use SMS instead, need to modify UserService
    // 4. Violates Single Responsibility - UserService manages its dependencies
    // 5. No flexibility - Cannot swap implementations at runtime
    // ============================================================

    /// <summary>
    /// Email service implementation - tightly coupled
    /// </summary>
    public class EmailService
    {
        public void SendEmail(string to, string message)
        {
            // Simulating email sending
            Console.WriteLine($"[EmailService] Sending email to {to}: {message}");
            // In real scenario, this would call SMTP server, API, etc.
        }
    }

    /// <summary>
    /// User service WITHOUT dependency injection
    /// PROBLEM: This class creates its own dependencies (EmailService)
    /// </summary>
    public class UserService
    {
        // Tightly coupled - directly instantiating EmailService
        private readonly EmailService _emailService;

        public UserService()
        {
            // PROBLEM: Hard-coded dependency creation
            // We cannot control which implementation is used
            // We cannot inject a mock for testing
            _emailService = new EmailService();
        }

        public void RegisterUser(string username, string email)
        {
            // Simulate user registration logic
            Console.WriteLine($"[UserService] Registering user: {username}");
            
            // Save to database (simulated)
            Console.WriteLine($"[UserService] Saving user to database...");
            
            // Send welcome email using tightly coupled EmailService
            _emailService.SendEmail(email, $"Welcome {username}!");
            
            Console.WriteLine($"[UserService] User {username} registered successfully!\n");
        }
    }

    /// <summary>
    /// Order service WITHOUT dependency injection
    /// Another example showing tight coupling
    /// </summary>
    public class OrderService
    {
        private readonly EmailService _emailService;

        public OrderService()
        {
            // Again, hard-coded dependency
            _emailService = new EmailService();
        }

        public void PlaceOrder(string orderId, string customerEmail)
        {
            Console.WriteLine($"[OrderService] Placing order: {orderId}");
            
            // Process order (simulated)
            Console.WriteLine($"[OrderService] Processing payment...");
            
            // Send confirmation email
            _emailService.SendEmail(customerEmail, $"Order {orderId} confirmed!");
            
            Console.WriteLine($"[OrderService] Order {orderId} placed successfully!\n");
        }
    }

    /// <summary>
    /// What if we want to add SMS notifications?
    /// We would need to modify EVERY class that uses notifications!
    /// </summary>
    public class SmsService
    {
        public void SendSms(string phoneNumber, string message)
        {
            Console.WriteLine($"[SmsService] Sending SMS to {phoneNumber}: {message}");
        }
    }

    // If we want to use SmsService, we need to:
    // 1. Modify UserService constructor
    // 2. Modify OrderService constructor
    // 3. Change all method calls
    // This violates Open/Closed Principle - classes should be open for extension, closed for modification
}
