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
            [$"{nameof(HealthReportEntry)}{nameof(HealthReportEntry.Status)}"] = entry.Value.Status.ToString(),
        };

        if (!string.IsNullOrEmpty(entry.Value.Description))
        {
            properties.Add($"{nameof(HealthReportEntry)}{nameof(HealthReportEntry.Description)}", entry.Value.Description);
        }

        if (entry.Value.Tags.Any())
        {
            MapTagsAsProperties(entry, properties);
        }

        var metrics = new Dictionary<string, double>();

        if (entry.Value.Data is not null)
        {
            MapData(entry.Value, properties, metrics);
        }

        MapMetrics(entry.Value, metrics);

        if (entry.Value.Exception is not null)
        {
            TrackException(entry.Value.Exception, properties, metrics);
        }

        client.TrackAvailability($"{options.ApplicationName}:{entry.Key}", DateTimeOffset.Now, entry.Value.Duration, options.RunLocation, entry.Value.Status == HealthStatus.Healthy, entry.Value.Description, properties, metrics);
    }

    private static void MapTagsAsProperties(KeyValuePair<string, HealthReportEntry> entry, Dictionary<string, string> properties)
    {
        var tags = JsonSerializer.Serialize(entry.Value.Tags);
        properties.Add($"{nameof(HealthReportEntry)}{nameof(HealthReportEntry.Tags)}", tags);
    }

    private void TrackException(Exception exception, Dictionary<string, string> properties, Dictionary<string, double> metrics)
    {
        properties.TryAdd("ExceptionId", Guid.NewGuid().ToString());
        properties.TryAdd("ExceptionType", exception.GetType().FullName!);
        properties.TryAdd("ExceptionMessage", exception.Message);
        properties.TryAdd("ExceptionStackTrace", exception.StackTrace!.ToString());

        client.TrackException(exception, properties, metrics);
    }

    private static void MapMetrics(HealthReportEntry entry, Dictionary<string, double> metrics)
    {
        metrics.Add($"{nameof(HealthReportEntry)}{nameof(HealthReportEntry.Duration)}", entry.Duration.TotalMilliseconds);
    }

    private static void MapData(HealthReportEntry entry, Dictionary<string, string> properties, Dictionary<string, double> metrics)
    {
        foreach (var data in entry.Data)
        {
            if (data.Value is double metric)
            {
                metrics.Add(data.Key, metric);
            }
            else if (data.Value is HealthReport entryReport)
            {
                MapHealthReportAsProperty(properties, data, entryReport);
            }
            else if (data.Value is string value)
            {
                properties.Add(data.Key, value);
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

    private static void MapHealthReportAsProperty(Dictionary<string, string> properties, KeyValuePair<string, object> data, HealthReport entryReport)
    {
        properties.Add(data.Key, entryReport.Status.ToString());

        foreach (var dataEntry in entryReport.Entries)
        {
            properties.Add($"{data.Key}:{dataEntry.Key}", dataEntry.Value.Status.ToString());
        }
    }
}
