using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

internal class HealthReportEntryConverter : JsonConverter<HealthReportEntry>
{
    public override HealthReportEntry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        HealthStatus status = HealthStatus.Unhealthy;
        string? description = null;
        TimeSpan duration = TimeSpan.Zero;
        Exception? exception = null;
        IReadOnlyDictionary<string, object>? data = null;
        IEnumerable<string>? tags = null;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (options.PropertyNameCaseInsensitive ? propertyName : propertyName?.ToLowerInvariant())
                {
                    case "Status":
                    case "status":
                        status = JsonSerializer.Deserialize<HealthStatus>(ref reader, options);
                        break;

                    case "Description":
                    case "description":
                        description = JsonSerializer.Deserialize<string>(ref reader, options);
                        break;

                    case "Duration":
                    case "duration":
                        duration = JsonSerializer.Deserialize<TimeSpan>(ref reader, options);
                        break;

                    case "Exception":
                    case "exception":
                        exception = JsonSerializer.Deserialize<Exception>(ref reader, options);
                        break;

                    case "Data":
                    case "data":
                        data = JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);
                        break;

                    case "Tags":
                    case "tags":
                        tags = JsonSerializer.Deserialize<string[]>(ref reader, options);
                        break;
                }
            }
        }

        return new HealthReportEntry(status, description, duration, exception, data, tags);
    }

    public override void Write(Utf8JsonWriter writer, HealthReportEntry value, JsonSerializerOptions options)
    {
        string ConvertName(string name)
            => options.PropertyNamingPolicy?.ConvertName(name) ?? name;

        var ignoreNullValues = options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull;

        writer.WriteStartObject();

        writer.WritePropertyName(ConvertName(nameof(value.Status)));
        JsonSerializer.Serialize(writer, value.Status, options);

        if (value.Description is not null || !ignoreNullValues)
        {
            writer.WritePropertyName(ConvertName(nameof(value.Description)));
            JsonSerializer.Serialize(writer, value.Description, options);
        }

        writer.WritePropertyName(ConvertName(nameof(value.Duration)));
        JsonSerializer.Serialize(writer, value.Duration, options);

        if (value.Data is not null || !ignoreNullValues)
        {
            writer.WritePropertyName(ConvertName(nameof(value.Data)));
            JsonSerializer.Serialize(writer, value.Data, options);
        }

        if (value.Exception is not null || !ignoreNullValues)
        {
            writer.WritePropertyName(ConvertName(nameof(value.Exception)));
            JsonSerializer.Serialize(writer, value.Exception, options);
        }

        if (value.Tags is not null || !ignoreNullValues)
        {
            writer.WritePropertyName(ConvertName(nameof(value.Tags)));
            JsonSerializer.Serialize(writer, value.Tags, options);
        }

        writer.WriteEndObject();
    }
}
