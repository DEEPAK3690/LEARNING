using Microsoft.AspNetCore.Mvc;
using DI_WebApp.Services;

namespace DI_WebApp.Controllers;

/// <summary>
/// Controller demonstrating service lifetime behaviors.
/// Each request creates a new controller instance, but injected services
/// follow their registered lifetimes (Transient/Scoped/Singleton).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LifetimeController : ControllerBase
{
    private readonly ITransientOperation _transientOperation1;
    private readonly ITransientOperation _transientOperation2;
    private readonly IScopedOperation _scopedOperation1;
    private readonly IScopedOperation _scopedOperation2;
    private readonly ISingletonOperation _singletonOperation1;
    private readonly ISingletonOperation _singletonOperation2;
    private readonly ILogger<LifetimeController> _logger;
    private readonly Guid _controllerId;

    public LifetimeController(
        ITransientOperation transientOperation1,
        ITransientOperation transientOperation2,
        IScopedOperation scopedOperation1,
        IScopedOperation scopedOperation2,
        ISingletonOperation singletonOperation1,
        ISingletonOperation singletonOperation2,
        ILogger<LifetimeController> logger)
    {
        _transientOperation1 = transientOperation1;
        _transientOperation2 = transientOperation2;
        _scopedOperation1 = scopedOperation1;
        _scopedOperation2 = scopedOperation2;
        _singletonOperation1 = singletonOperation1;
        _singletonOperation2 = singletonOperation2;
        _logger = logger;
        _controllerId = Guid.NewGuid();

        _logger.LogInformation($"LifetimeController created with ID: {_controllerId}");
    }

    /// <summary>
    /// Demonstrates service lifetime behaviors.
    /// GET /api/lifetime
    /// </summary>
    [HttpGet]
    public ActionResult<LifetimeInfo> GetLifetimeInfo()
    {
        var info = new LifetimeInfo
        {
            ControllerId = _controllerId,
            Transient1 = _transientOperation1.OperationId,
            Transient2 = _transientOperation2.OperationId,
            Scoped1 = _scopedOperation1.OperationId,
            Scoped2 = _scopedOperation2.OperationId,
            Singleton1 = _singletonOperation1.OperationId,
            Singleton2 = _singletonOperation2.OperationId
        };

        _logger.LogInformation("Lifetime Info: {@LifetimeInfo}", info);

        return Ok(info);
    }

    /// <summary>
    /// Additional endpoint to demonstrate lifetime behavior across multiple calls in same request.
    /// </summary>
    [HttpGet("detailed")]
    public ActionResult<DetailedLifetimeInfo> GetDetailedLifetimeInfo(
        [FromServices] ITransientOperation transientFromAction,
        [FromServices] IScopedOperation scopedFromAction,
        [FromServices] ISingletonOperation singletonFromAction)
    {
        var info = new DetailedLifetimeInfo
        {
            ControllerId = _controllerId,
            RequestTime = DateTime.UtcNow,
            
            // Constructor injected instances
            TransientConstructor1 = _transientOperation1.OperationId,
            TransientConstructor2 = _transientOperation2.OperationId,
            ScopedConstructor1 = _scopedOperation1.OperationId,
            ScopedConstructor2 = _scopedOperation2.OperationId,
            SingletonConstructor1 = _singletonOperation1.OperationId,
            SingletonConstructor2 = _singletonOperation2.OperationId,
            
            // Action injected instances
            TransientAction = transientFromAction.OperationId,
            ScopedAction = scopedFromAction.OperationId,
            SingletonAction = singletonFromAction.OperationId,
            
            Analysis = new LifetimeAnalysis
            {
                TransientsDiffer = _transientOperation1.OperationId != _transientOperation2.OperationId,
                ScopedsSame = _scopedOperation1.OperationId == _scopedOperation2.OperationId,
                SingletonsSame = _singletonOperation1.OperationId == _singletonOperation2.OperationId,
                ScopedMatchesAction = _scopedOperation1.OperationId == scopedFromAction.OperationId,
                SingletonMatchesAction = _singletonOperation1.OperationId == singletonFromAction.OperationId
            }
        };

        return Ok(info);
    }
}

public class LifetimeInfo
{
    public Guid ControllerId { get; set; }
    public Guid Transient1 { get; set; }
    public Guid Transient2 { get; set; }
    public Guid Scoped1 { get; set; }
    public Guid Scoped2 { get; set; }
    public Guid Singleton1 { get; set; }
    public Guid Singleton2 { get; set; }
}

public class DetailedLifetimeInfo
{
    public Guid ControllerId { get; set; }
    public DateTime RequestTime { get; set; }
    
    public Guid TransientConstructor1 { get; set; }
    public Guid TransientConstructor2 { get; set; }
    public Guid ScopedConstructor1 { get; set; }
    public Guid ScopedConstructor2 { get; set; }
    public Guid SingletonConstructor1 { get; set; }
    public Guid SingletonConstructor2 { get; set; }
    
    public Guid TransientAction { get; set; }
    public Guid ScopedAction { get; set; }
    public Guid SingletonAction { get; set; }
    
    public LifetimeAnalysis Analysis { get; set; } = new();
}

public class LifetimeAnalysis
{
    public bool TransientsDiffer { get; set; }
    public bool ScopedsSame { get; set; }
    public bool SingletonsSame { get; set; }
    public bool ScopedMatchesAction { get; set; }
    public bool SingletonMatchesAction { get; set; }
}
