using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

internal static class HealthReportJsonSerializerOptionsFactory
{
    private static readonly HealthReportEntryConverter _entryConverter = new();
    private static readonly HealthReportExceptionConverter _exceptionConverter = new();
    private static readonly HealthReportTimeSpanConverter _timeSpanConverter = new();
    private static readonly JsonStringEnumConverter _enumConverter = new();

    public static JsonSerializerOptions Create(JsonSerializerOptions options)
        => new(options)
        {
            Converters =
            {
                _entryConverter,
                _exceptionConverter,
                _timeSpanConverter,
                _enumConverter
            }
        };
}
