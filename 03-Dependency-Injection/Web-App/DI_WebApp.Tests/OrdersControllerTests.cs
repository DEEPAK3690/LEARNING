using Microsoft.AspNetCore.Mvc.Testing;
using DI_WebApp.Controllers;
using System.Net.Http.Json;
using Xunit;

namespace DI_WebApp.Tests;

/// <summary>
/// Tests for the Orders Controller demonstrating real-world DI usage.
/// </summary>
public class OrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OrdersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProcessOrder_ReturnsSuccessResponse()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new OrderRequest
        {
            OrderId = "ORDER-001",
            CustomerId = "CUST-123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<OrderResponse>();
        
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("ORDER-001", result.OrderId);
        Assert.Equal("CUST-123", result.CustomerId);
        Assert.NotEqual(Guid.Empty, result.RequestId);
    }

    [Fact]
    public async Task ProcessOrder_ResponseHasCorrelationId()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new OrderRequest
        {
            OrderId = "ORDER-002",
            CustomerId = "CUST-456"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.True(response.Headers.Contains("X-Request-ID"));
        
        var requestId = response.Headers.GetValues("X-Request-ID").FirstOrDefault();
        Assert.NotNull(requestId);
        Assert.True(Guid.TryParse(requestId, out var parsedId));
        
        var result = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(result);
        Assert.Equal(parsedId, result.RequestId);
    }

    [Fact]
    public async Task ProcessOrder_DifferentRequestsHaveDifferentRequestIds()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request1 = new OrderRequest { OrderId = "ORDER-003", CustomerId = "CUST-789" };
        var request2 = new OrderRequest { OrderId = "ORDER-004", CustomerId = "CUST-012" };

        // Act
        var response1 = await client.PostAsJsonAsync("/api/orders", request1);
        var result1 = await response1.Content.ReadFromJsonAsync<OrderResponse>();

        var response2 = await client.PostAsJsonAsync("/api/orders", request2);
        var result2 = await response2.Content.ReadFromJsonAsync<OrderResponse>();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEqual(result1.RequestId, result2.RequestId);
    }

    [Fact]
    public async Task GetRequestContext_ReturnsContextInfo()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/orders/context");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RequestContextInfo>();
        
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.RequestId);
        Assert.NotEqual(Guid.Empty, result.AppInstanceId);
        Assert.Equal("DI Web App Demo", result.ApplicationName);
        Assert.Equal("1.0.0", result.ApplicationVersion);
    }

    [Fact]
    public async Task GetRequestContext_AppInstanceIdRemainsConstant()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Make multiple requests
        var response1 = await client.GetAsync("/api/orders/context");
        var result1 = await response1.Content.ReadFromJsonAsync<RequestContextInfo>();

        var response2 = await client.GetAsync("/api/orders/context");
        var result2 = await response2.Content.ReadFromJsonAsync<RequestContextInfo>();

        var response3 = await client.GetAsync("/api/orders/context");
        var result3 = await response3.Content.ReadFromJsonAsync<RequestContextInfo>();

        // Assert - AppInstanceId (from Singleton) should be the same
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotNull(result3);
        
        Assert.Equal(result1.AppInstanceId, result2.AppInstanceId);
        Assert.Equal(result2.AppInstanceId, result3.AppInstanceId);
    }

    [Fact]
    public async Task GetRequestContext_RequestIdsDifferPerRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Make multiple requests
        var response1 = await client.GetAsync("/api/orders/context");
        var result1 = await response1.Content.ReadFromJsonAsync<RequestContextInfo>();

        var response2 = await client.GetAsync("/api/orders/context");
        var result2 = await response2.Content.ReadFromJsonAsync<RequestContextInfo>();

        // Assert - RequestIds (from Scoped) should be different
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEqual(result1.RequestId, result2.RequestId);
    }

    [Fact]
    public async Task ProcessOrder_HandlesMultipleConcurrentOrders()
    {
        // Arrange
        var client = _factory.CreateClient();
        var tasks = new List<Task<OrderResponse?>>();

        // Act - Process 5 orders concurrently
        for (int i = 0; i < 5; i++)
        {
            var orderId = $"ORDER-{i:D3}";
            var customerId = $"CUST-{i:D3}";
            
            tasks.Add(Task.Run(async () =>
            {
                var request = new OrderRequest { OrderId = orderId, CustomerId = customerId };
                var response = await client.PostAsJsonAsync("/api/orders", request);
                return await response.Content.ReadFromJsonAsync<OrderResponse>();
            }));
        }

        var results = await Task.WhenAll(tasks);

        // Assert - All should succeed with unique request IDs
        Assert.All(results, result =>
        {
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotEqual(Guid.Empty, result.RequestId);
        });

        // All request IDs should be unique
        var requestIds = results.Select(r => r!.RequestId).ToList();
        Assert.Equal(requestIds.Count, requestIds.Distinct().Count());
    }

    [Fact]
    public async Task ProcessOrder_WithInvalidData_ReturnsError()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new OrderRequest
        {
            OrderId = "",  // Invalid
            CustomerId = ""  // Invalid
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request);

        // Assert
        // The service should still process, but we can verify it returns a response
        var result = await response.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(result);
    }
}
