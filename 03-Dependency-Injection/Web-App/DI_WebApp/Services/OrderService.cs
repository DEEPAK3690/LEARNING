namespace DI_WebApp.Services;

/// <summary>
/// Service that processes orders and demonstrates mixed service lifetimes.
/// Uses Scoped RequestContext, Singleton AppConfiguration, and Transient NotificationService.
/// </summary>
public interface IOrderService
{
    Task<OrderResult> ProcessOrderAsync(string orderId, string customerId);
}

public class OrderService : IOrderService
{
    private readonly INotificationService _notificationService;
    private readonly IRequestContext _requestContext;
    private readonly IAppConfiguration _appConfiguration;
    private readonly ILogger<OrderService> _logger;
    private readonly Guid _instanceId;

    public OrderService(
        INotificationService notificationService,
        IRequestContext requestContext,
        IAppConfiguration appConfiguration,
        ILogger<OrderService> logger)
    {
        _notificationService = notificationService;
        _requestContext = requestContext;
        _appConfiguration = appConfiguration;
        _logger = logger;
        _instanceId = Guid.NewGuid();
        
        _logger.LogInformation($"OrderService created with ID: {_instanceId}");
    }

    public async Task<OrderResult> ProcessOrderAsync(string orderId, string customerId)
    {
        _logger.LogInformation($"Processing order {orderId} for customer {customerId}");
        _logger.LogInformation($"Request ID: {_requestContext.RequestId}");
        _logger.LogInformation($"App Instance: {_appConfiguration.InstanceId}");
        _logger.LogInformation($"OrderService Instance: {_instanceId}");

        // Simulate order processing
        await Task.Delay(200);

        // Send notification
        await _notificationService.SendAsync(
            customerId,
            $"Your order {orderId} has been processed successfully!");

        return new OrderResult
        {
            OrderId = orderId,
            CustomerId = customerId,
            RequestId = _requestContext.RequestId,
            ProcessedAt = DateTime.UtcNow,
            ServiceInstanceId = _instanceId,
            AppInstanceId = _appConfiguration.InstanceId
        };
    }
}

public class OrderResult
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public Guid RequestId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public Guid ServiceInstanceId { get; set; }
    public Guid AppInstanceId { get; set; }
}
