using Microsoft.AspNetCore.Mvc.Testing;
using DI_WebApp.Controllers;
using System.Net.Http.Json;
using Xunit;

namespace DI_WebApp.Tests;

/// <summary>
/// Tests for the Lifetime Controller demonstrating service lifetime behaviors.
/// </summary>
public class LifetimeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public LifetimeControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetLifetimeInfo_ReturnsOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/lifetime");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LifetimeInfo>();
        
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.ControllerId);
    }

    [Fact]
    public async Task GetLifetimeInfo_TransientServices_HaveDifferentIds()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/lifetime");
        var result = await response.Content.ReadFromJsonAsync<LifetimeInfo>();

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(result.Transient1, result.Transient2);
    }

    [Fact]
    public async Task GetLifetimeInfo_ScopedServices_HaveSameIdWithinRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/lifetime");
        var result = await response.Content.ReadFromJsonAsync<LifetimeInfo>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Scoped1, result.Scoped2);
    }

    [Fact]
    public async Task GetLifetimeInfo_SingletonServices_HaveSameIdWithinRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/lifetime");
        var result = await response.Content.ReadFromJsonAsync<LifetimeInfo>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.Singleton1, result.Singleton2);
    }

    [Fact]
    public async Task GetLifetimeInfo_ScopedServices_HaveDifferentIdsAcrossRequests()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Make two separate requests
        var response1 = await client.GetAsync("/api/lifetime");
        var result1 = await response1.Content.ReadFromJsonAsync<LifetimeInfo>();

        var response2 = await client.GetAsync("/api/lifetime");
        var result2 = await response2.Content.ReadFromJsonAsync<LifetimeInfo>();

        // Assert - Scoped services should differ across requests
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEqual(result1.Scoped1, result2.Scoped1);
    }

    [Fact]
    public async Task GetLifetimeInfo_SingletonServices_HaveSameIdsAcrossRequests()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Make two separate requests
        var response1 = await client.GetAsync("/api/lifetime");
        var result1 = await response1.Content.ReadFromJsonAsync<LifetimeInfo>();

        var response2 = await client.GetAsync("/api/lifetime");
        var result2 = await response2.Content.ReadFromJsonAsync<LifetimeInfo>();

        // Assert - Singleton services should be the same across requests
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Singleton1, result2.Singleton1);
        Assert.Equal(result1.Singleton2, result2.Singleton2);
    }

    [Fact]
    public async Task GetLifetimeInfo_ControllerInstances_AreDifferentAcrossRequests()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Make two separate requests
        var response1 = await client.GetAsync("/api/lifetime");
        var result1 = await response1.Content.ReadFromJsonAsync<LifetimeInfo>();

        var response2 = await client.GetAsync("/api/lifetime");
        var result2 = await response2.Content.ReadFromJsonAsync<LifetimeInfo>();

        // Assert - Controllers are created per request
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEqual(result1.ControllerId, result2.ControllerId);
    }

    [Fact]
    public async Task GetDetailedLifetimeInfo_ReturnsCorrectAnalysis()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/lifetime/detailed");
        var result = await response.Content.ReadFromJsonAsync<DetailedLifetimeInfo>();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Analysis);
        
        // Transients should differ
        Assert.True(result.Analysis.TransientsDiffer);
        
        // Scoped should be the same within request
        Assert.True(result.Analysis.ScopedsSame);
        Assert.True(result.Analysis.ScopedMatchesAction);
        
        // Singleton should be the same
        Assert.True(result.Analysis.SingletonsSame);
        Assert.True(result.Analysis.SingletonMatchesAction);
    }

    [Fact]
    public async Task GetDetailedLifetimeInfo_ResponseHasCorrelationId()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/lifetime/detailed");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.True(response.Headers.Contains("X-Request-ID"));
        
        var requestId = response.Headers.GetValues("X-Request-ID").FirstOrDefault();
        Assert.NotNull(requestId);
        Assert.True(Guid.TryParse(requestId, out _));
    }

    [Fact]
    public async Task MultipleParallelRequests_SingletonRemainsConsistent()
    {
        // Arrange
        var client = _factory.CreateClient();
        var tasks = new List<Task<LifetimeInfo?>>();

        // Act - Make 10 parallel requests
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var response = await client.GetAsync("/api/lifetime");
                return await response.Content.ReadFromJsonAsync<LifetimeInfo>();
            }));
        }

        var results = await Task.WhenAll(tasks);

        // Assert - All should have same singleton ID
        var singletonId = results[0]?.Singleton1;
        Assert.NotNull(singletonId);
        Assert.All(results, result =>
        {
            Assert.NotNull(result);
            Assert.Equal(singletonId, result.Singleton1);
            Assert.Equal(singletonId, result.Singleton2);
        });
    }
}
