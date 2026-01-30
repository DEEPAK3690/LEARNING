using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DI_WebApp.Services;
using Xunit;

namespace DI_WebApp.Tests;

/// <summary>
/// Unit tests for service classes demonstrating testability with DI.
/// </summary>
public class ServiceTests
{
    [Fact]
    public void TransientOperation_CreatesNewInstanceEachTime()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<ITransientOperation, TransientOperation>();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetRequiredService<ITransientOperation>();
        var instance2 = provider.GetRequiredService<ITransientOperation>();
        var instance3 = provider.GetRequiredService<ITransientOperation>();

        // Assert
        Assert.NotEqual(instance1.OperationId, instance2.OperationId);
        Assert.NotEqual(instance2.OperationId, instance3.OperationId);
        Assert.NotEqual(instance1.OperationId, instance3.OperationId);
    }

    [Fact]
    public void ScopedOperation_CreatesSameInstanceWithinScope()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IScopedOperation, ScopedOperation>();
        var provider = services.BuildServiceProvider();

        // Act
        using var scope = provider.CreateScope();
        var instance1 = scope.ServiceProvider.GetRequiredService<IScopedOperation>();
        var instance2 = scope.ServiceProvider.GetRequiredService<IScopedOperation>();

        // Assert
        Assert.Equal(instance1.OperationId, instance2.OperationId);
    }

    [Fact]
    public void ScopedOperation_CreatesDifferentInstancesAcrossScopes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IScopedOperation, ScopedOperation>();
        var provider = services.BuildServiceProvider();

        // Act
        Guid operationId1, operationId2;
        
        using (var scope1 = provider.CreateScope())
        {
            var instance1 = scope1.ServiceProvider.GetRequiredService<IScopedOperation>();
            operationId1 = instance1.OperationId;
        }

        using (var scope2 = provider.CreateScope())
        {
            var instance2 = scope2.ServiceProvider.GetRequiredService<IScopedOperation>();
            operationId2 = instance2.OperationId;
        }

        // Assert
        Assert.NotEqual(operationId1, operationId2);
    }

    [Fact]
    public void SingletonOperation_CreatesSameInstanceAlways()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ISingletonOperation, SingletonOperation>();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetRequiredService<ISingletonOperation>();
        var instance2 = provider.GetRequiredService<ISingletonOperation>();
        
        Guid operationId3;
        using (var scope = provider.CreateScope())
        {
            var instance3 = scope.ServiceProvider.GetRequiredService<ISingletonOperation>();
            operationId3 = instance3.OperationId;
        }

        // Assert
        Assert.Equal(instance1.OperationId, instance2.OperationId);
        Assert.Equal(instance1.OperationId, operationId3);
    }

    [Fact]
    public void RequestContext_TracksRequestInformation()
    {
        // Arrange
        var context = new RequestContext();

        // Act
        context.UserId = "testuser";
        context.Data["key1"] = "value1";
        context.Data["key2"] = 42;

        // Assert
        Assert.NotEqual(Guid.Empty, context.RequestId);
        Assert.Equal("testuser", context.UserId);
        Assert.Equal("value1", context.Data["key1"]);
        Assert.Equal(42, context.Data["key2"]);
        Assert.True(context.RequestTime <= DateTime.UtcNow);
    }

    [Fact]
    public void AppConfiguration_HasCorrectDefaultValues()
    {
        // Arrange & Act
        var config = new AppConfiguration();

        // Assert
        Assert.NotEqual(Guid.Empty, config.InstanceId);
        Assert.Equal("DI Web App Demo", config.ApplicationName);
        Assert.Equal("1.0.0", config.Version);
        Assert.True(config.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void EmailNotificationService_GetServiceInfo_ReturnsCorrectInfo()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient<INotificationService, EmailNotificationService>();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetRequiredService<INotificationService>();
        var info = service.GetServiceInfo();

        // Assert
        Assert.Contains("EmailNotificationService", info);
        Assert.Contains("Instance:", info);
    }

    [Fact]
    public async Task EmailNotificationService_SendAsync_ExecutesSuccessfully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient<INotificationService, EmailNotificationService>();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<INotificationService>();

        // Act & Assert - Should not throw
        await service.SendAsync("test@example.com", "Test message");
    }

    [Fact]
    public void SmsNotificationService_GetServiceInfo_ReturnsCorrectInfo()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient<INotificationService, SmsNotificationService>();
        var provider = services.BuildServiceProvider();

        // Act
        var service = provider.GetRequiredService<INotificationService>();
        var info = service.GetServiceInfo();

        // Assert
        Assert.Contains("SmsNotificationService", info);
        Assert.Contains("Instance:", info);
    }

    [Fact]
    public async Task SmsNotificationService_SendAsync_ExecutesSuccessfully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient<INotificationService, SmsNotificationService>();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<INotificationService>();

        // Act & Assert - Should not throw
        await service.SendAsync("+1234567890", "Test message");
    }

    [Fact]
    public void NotificationService_DifferentImplementations_HaveDifferentInstanceIds()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var provider = services.BuildServiceProvider();

        var emailService1 = new EmailNotificationService(
            provider.GetRequiredService<ILogger<EmailNotificationService>>());
        var emailService2 = new EmailNotificationService(
            provider.GetRequiredService<ILogger<EmailNotificationService>>());

        // Act
        var info1 = emailService1.GetServiceInfo();
        var info2 = emailService2.GetServiceInfo();

        // Assert - Different instances should have different IDs
        Assert.NotEqual(info1, info2);
    }
}
