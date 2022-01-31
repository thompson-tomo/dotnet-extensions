using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks.ApplicationInsights;
using Microsoft.Extensions.Options;
using Xunit;

namespace Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights.Tests;

public class ApplicationInsightsHealthCheckPublisherServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationInsightsPublisherShouldRegisterIHealthCheckPublisher()
    {
        var sut = new ServiceCollection().AddOptions();

        sut.AddHealthChecks().AddApplicationInsightsPublisher();

        sut.Any(x => x.ServiceType == typeof(IHealthCheckPublisher) && x.ImplementationType == typeof(ApplicationInsightsHealthCheckPublisher)).Should().BeTrue();
    }

    [Fact]
    public void AfterAddApplicationInsightsPublisherShouldBeAbleToGetApplicationInsightsHealthCheckPublisherOptions()
    {
        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
            .AddOptions()
            .AddHealthChecks()
                .AddApplicationInsightsPublisher()
                .Services;

        using var sut = services.BuildServiceProvider();

        var result = sut.GetService<IOptions<ApplicationInsightsHealthCheckPublisherOptions>>();

        result?.Value.Should().NotBeNull();
    }
}
