using System.Collections;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Diagnostics.HealthChecks.ApplicationInsights;

public class ApplicationInsightsHealthCheckPublisher : IHealthCheckPublisher
{
    private readonly TelemetryClient client;
    private readonly ApplicationInsightsHealthCheckPublisherOptions options;

    public ApplicationInsightsHealthCheckPublisher(TelemetryClient client, IOptions<ApplicationInsightsHealthCheckPublisherOptions> options)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
        this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        _ = report ?? throw new ArgumentNullException(nameof(report));

        client.TrackEvent(nameof(HealthReport),
            new Dictionary<string, string>
            {
                [nameof(ApplicationInsightsHealthCheckPublisherOptions.RunLocation)] = options.RunLocation,
                [nameof(ApplicationInsightsHealthCheckPublisherOptions.ApplicationName)] = options.ApplicationName,
                [nameof(ApplicationInsightsHealthCheckPublisherOptions.EnvironmentName)] = options.EnvironmentName,
                [nameof(HealthReport.Status)] = report.Status.ToString(),
                [nameof(HealthReport.Entries)] = string.Join(",", report.Entries.Select(x => x.Key))
            },
            new Dictionary<string, double>
            {
                [nameof(HealthReport.TotalDuration)] = report.TotalDuration.TotalMilliseconds
            });

        foreach (var entry in report.Entries)
        {
            TrackAvailability(entry);
        }

        return Task.CompletedTask;
    }

    private void TrackAvailability(KeyValuePair<string, HealthReportEntry> entry)
    {
        var properties = new Dictionary<string, string>
        {
            [nameof(ApplicationInsightsHealthCheckPublisherOptions.EnvironmentName)] = options.EnvironmentName,
        };

        var metrics = new Dictionary<string, double>();

        if (entry.Value.Data is not null)
        {
            foreach (var data in entry.Value.Data)
            {
                if (data.Value is double metric)
                {
                    metrics.Add(data.Key, metric);
                }
                else if (data.Value is HealthReport entryReport)
                {
                    properties.Add(data.Key, HealthReportSerializer.Serialize(entryReport));
                }
                else if (data.Value is IEnumerable enumerable)
                {
                    properties.Add(data.Key, JsonSerializer.Serialize(enumerable));
                }
                else
                {
                    properties.Add(data.Key, data.Value?.ToString() ?? string.Empty);
                }
            }
        }

        metrics.Add($"{nameof(HealthReportEntry)}{nameof(HealthReportEntry.Duration)}", entry.Value.Duration.TotalMilliseconds);

        if (entry.Value.Exception is not null)
        {
            client.TrackException(entry.Value.Exception, properties, metrics);
        }

        client.TrackAvailability($"{options.ApplicationName}:{entry.Key}", DateTimeOffset.Now, entry.Value.Duration, options.RunLocation, entry.Value.Status == HealthStatus.Healthy, entry.Value.Description, properties, metrics);
    }
}
