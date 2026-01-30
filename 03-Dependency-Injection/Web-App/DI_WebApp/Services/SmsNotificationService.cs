namespace DI_WebApp.Services;

/// <summary>
/// SMS notification implementation.
/// Alternative implementation demonstrating the power of DI.
/// </summary>
public class SmsNotificationService : INotificationService
{
    private readonly ILogger<SmsNotificationService> _logger;
    private readonly Guid _instanceId;

    public SmsNotificationService(ILogger<SmsNotificationService> logger)
    {
        _logger = logger;
        _instanceId = Guid.NewGuid();
        _logger.LogInformation($"SmsNotificationService created with ID: {_instanceId}");
    }

    public async Task SendAsync(string recipient, string message)
    {
        _logger.LogInformation($"Sending SMS to {recipient}: {message}");
        // Simulate async operation
        await Task.Delay(150);
        _logger.LogInformation($"SMS sent successfully to {recipient}");
    }

    public string GetServiceInfo()
    {
        return $"SmsNotificationService (Instance: {_instanceId})";
    }
}
