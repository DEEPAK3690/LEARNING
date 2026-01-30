namespace DI_WebApp.Services;

/// <summary>
/// Interface for application-wide configuration.
/// Demonstrates Singleton service lifetime.
/// </summary>
public interface IAppConfiguration
{
    Guid InstanceId { get; }
    DateTime CreatedAt { get; }
    string ApplicationName { get; }
    string Version { get; }
}

/// <summary>
/// Implementation of AppConfiguration - should be registered as Singleton.
/// Single instance shared across all requests and services.
/// </summary>
public class AppConfiguration : IAppConfiguration
{
    public Guid InstanceId { get; }
    public DateTime CreatedAt { get; }
    public string ApplicationName { get; }
    public string Version { get; }

    public AppConfiguration()
    {
        InstanceId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        ApplicationName = "DI Web App Demo";
        Version = "1.0.0";
    }
}
