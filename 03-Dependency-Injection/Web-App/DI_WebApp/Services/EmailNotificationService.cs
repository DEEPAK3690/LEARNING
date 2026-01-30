namespace DI_WebApp.Services;

/// <summary>
/// Email notification implementation.
/// Demonstrates dependency injection in a real-world scenario.
/// </summary>
public class EmailNotificationService : INotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly Guid _instanceId;

    public EmailNotificationService(ILogger<EmailNotificationService> logger)
    {
        _logger = logger;
        _instanceId = Guid.NewGuid();
        _logger.LogInformation($"EmailNotificationService created with ID: {_instanceId}");
    }

    public async Task SendAsync(string recipient, string message)
    {
        _logger.LogInformation($"Sending EMAIL to {recipient}: {message}");
        // Simulate async operation
        await Task.Delay(100);
        _logger.LogInformation($"EMAIL sent successfully to {recipient}");
    }

    public string GetServiceInfo()
    {
        return $"EmailNotificationService (Instance: {_instanceId})";
    }
}
