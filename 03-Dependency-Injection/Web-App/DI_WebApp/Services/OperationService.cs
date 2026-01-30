namespace DI_WebApp.Services;

/// <summary>
/// Base implementation of IOperationService with a unique OperationId.
/// Each instance gets a new GUID to demonstrate when new instances are created.
/// </summary>
public class OperationService : IOperationService
{
    public Guid OperationId { get; }

    public OperationService()
    {
        OperationId = Guid.NewGuid();
    }
}

/// <summary>
/// Transient operation - new instance every time it's requested.
/// </summary>
public class TransientOperation : OperationService, ITransientOperation { }

/// <summary>
/// Scoped operation - one instance per HTTP request/scope.
/// </summary>
public class ScopedOperation : OperationService, IScopedOperation { }

/// <summary>
/// Singleton operation - single instance for application lifetime.
/// </summary>
public class SingletonOperation : OperationService, ISingletonOperation { }
