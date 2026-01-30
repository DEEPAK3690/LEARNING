using Xunit;
using Moq;
using DI_Demo.WithDI;

namespace DI_Demo.Tests
{
    /// <summary>
    /// Tests for services WITH Dependency Injection
    /// These tests demonstrate how DI makes testing easy and effective
    /// </summary>
    public class WithDI_Tests
    {
        [Fact]
        public void UserService_RegisterUser_SendsNotification()
        {
            // ✅ BENEFIT 1: We can create a MOCK of INotificationService
            var mockNotificationService = new Mock<INotificationService>();
            
            // ✅ BENEFIT 2: We can INJECT the mock into the service
            var userService = new UserService(mockNotificationService.Object);
            
            // Act - Call the method we want to test
            userService.RegisterUser("TestUser", "test@example.com");
            
            // ✅ BENEFIT 3: We can VERIFY that the notification was sent
            // Check that SendNotification was called exactly once
            mockNotificationService.Verify(
                x => x.SendNotification("test@example.com", "Welcome TestUser!"),
                Times.Once,
                "Expected SendNotification to be called once with correct parameters"
            );
            
            // ✅ This is a STRONG test that verifies actual behavior!
        }

        [Fact]
        public void UserService_RegisterUser_WithNullRecipient_StillCallsNotification()
        {
            // Arrange
            var mockNotificationService = new Mock<INotificationService>();
            var userService = new UserService(mockNotificationService.Object);
            
            // Act
            userService.RegisterUser("TestUser", null);
            
            // Assert - Verify it was called even with null recipient
            mockNotificationService.Verify(
                x => x.SendNotification(null, "Welcome TestUser!"),
                Times.Once
            );
        }

        [Fact]
        public void UserService_Constructor_ThrowsException_WhenServiceIsNull()
        {
            // ✅ BENEFIT 4: We can test error scenarios
            
            // Assert that constructor throws ArgumentNullException
            Assert.Throws<ArgumentNullException>(() => new UserService(null));
        }

        [Fact]
        public void OrderService_PlaceOrder_SendsConfirmation()
        {
            // Arrange
            var mockNotificationService = new Mock<INotificationService>();
            var orderService = new OrderService(mockNotificationService.Object);
            
            // Act
            orderService.PlaceOrder("ORD-123", "customer@example.com");
            
            // Assert - Verify notification was sent with correct message
            mockNotificationService.Verify(
                x => x.SendNotification("customer@example.com", "Order ORD-123 confirmed!"),
                Times.Once
            );
        }

        [Fact]
        public void PaymentService_ProcessPayment_SendsNotificationAndLogsMessages()
        {
            // ✅ BENEFIT 5: We can inject MULTIPLE mocks
            var mockNotificationService = new Mock<INotificationService>();
            var mockLoggerService = new Mock<ILoggerService>();
            
            var paymentService = new PaymentService(
                mockNotificationService.Object,
                mockLoggerService.Object
            );
            
            // Act
            paymentService.ProcessPayment("PAY-456", "customer@example.com", 99.99m);
            
            // ✅ BENEFIT 6: We can verify MULTIPLE interactions
            
            // Verify notification was sent
            mockNotificationService.Verify(
                x => x.SendNotification("customer@example.com", "Payment of $99.99 processed successfully!"),
                Times.Once
            );
            
            // Verify logging happened twice (start and end)
            mockLoggerService.Verify(
                x => x.Log(It.IsAny<string>()),
                Times.Exactly(2)
            );
            
            // Verify specific log messages
            mockLoggerService.Verify(
                x => x.Log("Processing payment PAY-456 for amount $99.99"),
                Times.Once
            );
            
            mockLoggerService.Verify(
                x => x.Log("Payment PAY-456 processed successfully"),
                Times.Once
            );
        }

        [Fact]
        public void UserService_CanUse_EmailNotificationService()
        {
            // ✅ BENEFIT 7: We can test with REAL implementations when needed
            var emailService = new EmailNotificationService();
            var userService = new UserService(emailService);
            
            // This will use the real EmailNotificationService
            userService.RegisterUser("TestUser", "test@example.com");
            
            // No exception means it worked!
            Assert.True(true);
        }

        [Fact]
        public void UserService_CanUse_SmsNotificationService()
        {
            // ✅ BENEFIT 8: We can easily test with DIFFERENT implementations
            var smsService = new SmsNotificationService();
            var userService = new UserService(smsService);
            
            // Same UserService, different notification mechanism!
            userService.RegisterUser("TestUser", "+1-555-0123");
            
            Assert.True(true);
        }

        [Fact]
        public void UserService_CanUse_PushNotificationService()
        {
            // Another implementation - no changes needed to UserService!
            var pushService = new PushNotificationService();
            var userService = new UserService(pushService);
            
            userService.RegisterUser("TestUser", "device-token-abc");
            
            Assert.True(true);
        }

        [Fact]
        public void NotificationService_SimulateFailure()
        {
            // ✅ BENEFIT 9: We can SIMULATE FAILURES to test error handling
            
            var mockNotificationService = new Mock<INotificationService>();
            
            // Setup the mock to throw an exception
            mockNotificationService
                .Setup(x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InvalidOperationException("Notification service is down"));
            
            var userService = new UserService(mockNotificationService.Object);
            
            // Verify that the exception propagates
            // In a real app, UserService might catch and handle this
            Assert.Throws<InvalidOperationException>(() => 
                userService.RegisterUser("TestUser", "test@example.com")
            );
        }

        [Fact]
        public void PaymentService_LogsCorrectSequence()
        {
            // ✅ BENEFIT 10: We can verify the ORDER of operations
            
            var mockNotificationService = new Mock<INotificationService>();
            var mockLoggerService = new Mock<ILoggerService>();
            var paymentService = new PaymentService(mockNotificationService.Object, mockLoggerService.Object);
            
            var sequence = new MockSequence();
            
            // Verify methods are called in correct order
            mockLoggerService.InSequence(sequence)
                .Setup(x => x.Log("Processing payment PAY-789 for amount $50"));
            
            mockLoggerService.InSequence(sequence)
                .Setup(x => x.Log("Payment PAY-789 processed successfully"));
            
            mockNotificationService.InSequence(sequence)
                .Setup(x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()));
            
            // Act
            paymentService.ProcessPayment("PAY-789", "test@example.com", 50m);
            
            // If sequence is wrong, test will fail
        }

        [Fact]
        public void NotificationService_VerifyParameters()
        {
            // ✅ BENEFIT 11: We can use FLEXIBLE matchers for verification
            
            var mockNotificationService = new Mock<INotificationService>();
            var userService = new UserService(mockNotificationService.Object);
            
            userService.RegisterUser("TestUser", "test@example.com");
            
            // Verify with flexible matchers
            mockNotificationService.Verify(
                x => x.SendNotification(
                    It.Is<string>(s => s.Contains("@")),  // Email contains @
                    It.Is<string>(s => s.StartsWith("Welcome"))  // Message starts with Welcome
                ),
                Times.Once
            );
        }

        [Fact]
        public void UserService_MultipleUsers_MultipleNotifications()
        {
            // ✅ BENEFIT 12: We can verify behavior across multiple calls
            
            var mockNotificationService = new Mock<INotificationService>();
            var userService = new UserService(mockNotificationService.Object);
            
            // Register multiple users
            userService.RegisterUser("User1", "user1@example.com");
            userService.RegisterUser("User2", "user2@example.com");
            userService.RegisterUser("User3", "user3@example.com");
            
            // Verify SendNotification was called exactly 3 times
            mockNotificationService.Verify(
                x => x.SendNotification(It.IsAny<string>(), It.IsAny<string>()),
                Times.Exactly(3)
            );
        }

        /// <summary>
        /// Custom fake implementation for testing specific scenarios
        /// </summary>
        private class FakeNotificationService : INotificationService
        {
            public List<(string Recipient, string Message)> SentNotifications { get; } = new();

            public void SendNotification(string recipient, string message)
            {
                SentNotifications.Add((recipient, message));
            }
        }

        [Fact]
        public void UserService_WithCustomFake_TracksNotifications()
        {
            // ✅ BENEFIT 13: We can create custom fakes with specific behavior
            
            var fakeNotificationService = new FakeNotificationService();
            var userService = new UserService(fakeNotificationService);
            
            userService.RegisterUser("TestUser", "test@example.com");
            
            // Assert using the fake's tracking
            Assert.Single(fakeNotificationService.SentNotifications);
            Assert.Equal("test@example.com", fakeNotificationService.SentNotifications[0].Recipient);
            Assert.Equal("Welcome TestUser!", fakeNotificationService.SentNotifications[0].Message);
        }
    }

    /// <summary>
    /// SUMMARY OF BENEFITS TESTING WITH DI:
    /// 
    /// 1. ✅ Can inject mock/fake implementations
    /// 2. ✅ Can verify method calls and parameters
    /// 3. ✅ Can test error scenarios by simulating failures
    /// 4. ✅ Tests are isolated - no external dependencies
    /// 5. ✅ Tests run fast - no network calls or I/O
    /// 6. ✅ Tests are free - no costs for external services
    /// 7. ✅ Can easily test different configurations
    /// 8. ✅ Strong assertions - verify actual behavior
    /// 9. ✅ Can verify sequence of operations
    /// 10. ✅ Can use flexible matchers for verification
    /// 11. ✅ Can test with real or mock implementations
    /// 12. ✅ Easy to test in isolation
    /// 13. ✅ Can create custom test doubles for specific scenarios
    /// 
    /// DI makes code testable, maintainable, and flexible!
    /// </summary>
}
