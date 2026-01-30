namespace DI_WebApp.Services;

/// <summary>
/// Interface for sending notifications.
/// Demonstrates loose coupling and dependency injection.
/// </summary>
public interface INotificationService
{
    Task SendAsync(string recipient, string message);
    string GetServiceInfo();
}
