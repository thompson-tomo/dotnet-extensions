using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

public class HealthReportConverter : JsonConverter<HealthReport>
{
    public override HealthReport Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var healthReportOptions = HealthReportJsonSerializerOptionsFactory.Create(options);
        Dictionary<string, HealthReportEntry>? entries = null;
        var totalDuration = TimeSpan.Zero;
        HealthStatus? status = null;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (healthReportOptions.PropertyNameCaseInsensitive ? propertyName : propertyName?.ToLowerInvariant())
                {
                    case "Status":
                    case "status":
                        status = JsonSerializer.Deserialize<HealthStatus>(ref reader, healthReportOptions);
                        break;

                    case "TotalDuration":
                    case "totalDuration":
                    case "totalduration":
                        totalDuration = JsonSerializer.Deserialize<TimeSpan>(ref reader, healthReportOptions);
                        break;

                    case "Entries":
                    case "entries":
                        entries = JsonSerializer.Deserialize<Dictionary<string, HealthReportEntry>>(ref reader, healthReportOptions);
                        break;
                }
            }
        }

        var report = status is null
            ? new HealthReport(entries ?? new(), totalDuration)
            : new HealthReport(entries ?? new(), status.Value, totalDuration);

        return report;
    }

    public override void Write(Utf8JsonWriter writer, HealthReport value, JsonSerializerOptions options)
    {
        string ConvertName(string name)
            => options.PropertyNamingPolicy?.ConvertName(name) ?? name;

        var healthReportOptions = HealthReportJsonSerializerOptionsFactory.Create(options);

        writer.WriteStartObject();

        writer.WritePropertyName(ConvertName(nameof(value.Status)));
        JsonSerializer.Serialize(writer, value.Status, healthReportOptions);

        writer.WritePropertyName(ConvertName(nameof(value.TotalDuration)));
        JsonSerializer.Serialize(writer, value.TotalDuration, healthReportOptions);

        writer.WritePropertyName(ConvertName("Timestamp"));
        JsonSerializer.Serialize(writer, DateTimeOffset.Now, healthReportOptions);

        writer.WritePropertyName(ConvertName(nameof(value.Entries)));
        JsonSerializer.Serialize(writer, value.Entries, healthReportOptions);

        writer.WriteEndObject();
    }
}
