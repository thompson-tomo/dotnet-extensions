using AutoFixture;
using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks.ApplicationInsights;
using Microsoft.Extensions.Options;
using Xunit;

namespace Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights.Tests;

public class ApplicationInsightsHealthCheckPublisherTests
{
    [Fact]
    public void PublishAsyncDoesNotThrow()
    {
        var fixture = new Fixture();

        using var configuration = TelemetryConfiguration.CreateDefault();
        configuration.DisableTelemetry = true;
        var client = new TelemetryClient(configuration);

        var entries = new Dictionary<string, HealthReportEntry>
        {
            [fixture.Create<string>()] = new(fixture.Create<HealthStatus>(), fixture.Create<string>(), fixture.Create<TimeSpan>(), null, new Dictionary<string, object>())
        };

        var sut = new ApplicationInsightsHealthCheckPublisher(client, Options.Create(new ApplicationInsightsHealthCheckPublisherOptions()));

        var report = new HealthReport(entries, fixture.Create<TimeSpan>());

        var act = sut.Invoking(x => x.PublishAsync(report, CancellationToken.None));

        act.Should().NotThrowAsync();
    }
}
