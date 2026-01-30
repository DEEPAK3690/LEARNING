using Microsoft.AspNetCore.Mvc;
using DI_WebApp.Services;

namespace DI_WebApp.Controllers;

/// <summary>
/// Controller demonstrating real-world DI usage with business services.
/// Shows how different services work together with different lifetimes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IRequestContext _requestContext;
    private readonly IAppConfiguration _appConfiguration;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderService orderService,
        IRequestContext requestContext,
        IAppConfiguration appConfiguration,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _requestContext = requestContext;
        _appConfiguration = appConfiguration;
        _logger = logger;
    }

    /// <summary>
    /// Process an order - demonstrates service collaboration.
    /// POST /api/orders
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> ProcessOrder([FromBody] OrderRequest request)
    {
        try
        {
            _logger.LogInformation($"Processing order request for customer: {request.CustomerId}");
            
            var result = await _orderService.ProcessOrderAsync(request.OrderId, request.CustomerId);

            var response = new OrderResponse
            {
                Success = true,
                OrderId = result.OrderId,
                CustomerId = result.CustomerId,
                ProcessedAt = result.ProcessedAt,
                RequestId = _requestContext.RequestId,
                Message = "Order processed successfully"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order");
            return StatusCode(500, new OrderResponse
            {
                Success = false,
                Message = $"Error processing order: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Get request context information.
    /// GET /api/orders/context
    /// </summary>
    [HttpGet("context")]
    public ActionResult<RequestContextInfo> GetRequestContext()
    {
        var info = new RequestContextInfo
        {
            RequestId = _requestContext.RequestId,
            RequestTime = _requestContext.RequestTime,
            UserId = _requestContext.UserId,
            ApplicationName = _appConfiguration.ApplicationName,
            ApplicationVersion = _appConfiguration.Version,
            AppInstanceId = _appConfiguration.InstanceId,
            AppCreatedAt = _appConfiguration.CreatedAt
        };

        return Ok(info);
    }
}

public class OrderRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
}

public class OrderResponse
{
    public bool Success { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public Guid RequestId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class RequestContextInfo
{
    public Guid RequestId { get; set; }
    public DateTime RequestTime { get; set; }
    public string? UserId { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
    public string ApplicationVersion { get; set; } = string.Empty;
    public Guid AppInstanceId { get; set; }
    public DateTime AppCreatedAt { get; set; }
}
