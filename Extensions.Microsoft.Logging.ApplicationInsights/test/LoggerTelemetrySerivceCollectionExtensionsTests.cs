using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Extensions.Microsoft.Logging.ApplicationInsights.Tests;

public class LoggerTelemetrySerivceCollectionExtensionsTests
{
    [Fact]
    public void CanResolveILoggerTelemetryOfTFromServiceProvider()
    {
        using var sut = CreateServiceProvider();

        var result = sut.GetService<ILoggerTelemetry<LoggerTelemetrySerivceCollectionExtensionsTests>>();

        result.Should().NotBeNull();
    }

    [Fact]
    public void CanResolveILoggerTelemetryFactoryFromServiceProvider()
    {
        using var sut = CreateServiceProvider();

        var result = sut.GetService<ILoggerTelemetryFactory>();

        result.Should().NotBeNull();
    }

    [Fact]
    public void CanResolveILoggerTelemetryFromLoggerTelemetryFactory()
    {
        using var serviceProvider = CreateServiceProvider();
        var sut = serviceProvider.GetRequiredService<ILoggerTelemetryFactory>();

        var result = sut.CreateLogger<LoggerTelemetrySerivceCollectionExtensionsTests>();

        result.Should().NotBeNull();
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTransient(_ => TelemetryClientFixture.Create());
        services.AddLoggerTelemetry();
        return services.BuildServiceProvider();
    }
}
