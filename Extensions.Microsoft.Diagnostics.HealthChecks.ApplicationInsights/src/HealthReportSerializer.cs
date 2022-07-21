using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.Diagnostics.HealthChecks.ApplicationInsights;

public static class HealthReportSerializer
{
    private static readonly JsonSerializerOptions __options = CreateJsonSerializerOptions();

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            MaxDepth = 100
        };

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new HealthReportConverter());

        return options;
    }

    public static string Serialize(HealthReport report)
        => JsonSerializer.Serialize(report, __options);

    public static Task SerializeAsync(HealthReport report, Stream stream, CancellationToken cancellationToken = default)
        => JsonSerializer.SerializeAsync(stream, report, __options, cancellationToken);

    public static HealthReport Deserialize(string json)
        => JsonSerializer.Deserialize<HealthReport>(json, __options)!;
}
