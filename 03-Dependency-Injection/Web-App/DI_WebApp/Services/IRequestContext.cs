namespace DI_WebApp.Services;

/// <summary>
/// Interface for tracking request-specific information.
/// Demonstrates Scoped service lifetime - one instance per HTTP request.
/// </summary>
public interface IRequestContext
{
    Guid RequestId { get; }
    DateTime RequestTime { get; }
    string? UserId { get; set; }
    Dictionary<string, object> Data { get; }
}

/// <summary>
/// Implementation of RequestContext - should be registered as Scoped.
/// Tracks information throughout a single HTTP request.
/// </summary>
public class RequestContext : IRequestContext
{
    public Guid RequestId { get; }
    public DateTime RequestTime { get; }
    public string? UserId { get; set; }
    public Dictionary<string, object> Data { get; }

    public RequestContext()
    {
        RequestId = Guid.NewGuid();
        RequestTime = DateTime.UtcNow;
        Data = new Dictionary<string, object>();
    }
}
