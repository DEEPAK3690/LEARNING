namespace DI_WebApp.Services;

/// <summary>
/// Base interface for demonstrating service lifetimes.
/// Each service has a unique OperationId to track instances.
/// </summary>
public interface IOperationService
{
    Guid OperationId { get; }
}

/// <summary>
/// Marker interface for Transient services.
/// Transient services are created each time they're requested.
/// </summary>
public interface ITransientOperation : IOperationService { }

/// <summary>
/// Marker interface for Scoped services.
/// Scoped services are created once per HTTP request.
/// </summary>
public interface IScopedOperation : IOperationService { }

/// <summary>
/// Marker interface for Singleton services.
/// Singleton services are created once and shared across all requests.
/// </summary>
public interface ISingletonOperation : IOperationService { }
