using System.Globalization;
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
        options.Converters.Add(new TimeSpanConverter());
        options.Converters.Add(new ExceptionConverter());
        options.Converters.Add(new HealthReportConverter());
        options.Converters.Add(new HealthReportEntryConverter());

        return options;
    }

    public static string Serialize(HealthReport report)
        => JsonSerializer.Serialize(report, __options);

    public static Task SerializeAsync(HealthReport report, Stream stream, CancellationToken cancellationToken = default)
        => JsonSerializer.SerializeAsync(stream, report, __options, cancellationToken);

    public static HealthReport Deserialize(string json)
        => JsonSerializer.Deserialize<HealthReport>(json, __options)!;

    private class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => TimeSpan.Parse(reader.GetString()!, CultureInfo.InvariantCulture);

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }

    private class ExceptionConverter : JsonConverter<Exception>
    {
        public override Exception Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            string? message = null;
            string? stackTrace = null;
            string? source = null;
            Exception? innerException = null;
            string? type = null;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (options.PropertyNameCaseInsensitive ? propertyName : propertyName?.ToLowerInvariant())
                    {
                        case "Type":
                        case "type":
                            type = reader.GetString();
                            break;

                        case "Message":
                        case "message":
                            message = reader.GetString();
                            break;

                        case "Source":
                        case "source":
                            source = reader.GetString();
                            break;

                        case "StackTrace":
                        case "stackTrace":
                        case "stacktrace":
                            stackTrace = reader.GetString();
                            break;

                        case "InnerException":
                        case "innerException":
                        case "innerexception":
                            innerException = JsonSerializer.Deserialize<Exception>(ref reader, options);
                            break;
                    }
                }
            }

            type ??= typeof(Exception).AssemblyQualifiedName!;

            return new JsonSerializedException(type, message, source, stackTrace, innerException);
        }

        public override void Write(Utf8JsonWriter writer, Exception value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(options);

            string ConvertName(string name)
                => options.PropertyNamingPolicy?.ConvertName(name) ?? name;

            var ignoreNullValues = options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull;

            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            if (value is JsonSerializedException jse)
            {
                writer.WriteString(ConvertName(nameof(Type)), jse.Type);
            }
            else
            {
                writer.WriteString(ConvertName(nameof(Type)), value.GetType().AssemblyQualifiedName);
            }

            if (value.Message is not null || !ignoreNullValues)
            {
                writer.WriteString(ConvertName(nameof(value.Message)), value.Message);
            }

            writer.WriteString(ConvertName(nameof(value.StackTrace)), value.StackTrace);

            if (value.Source is not null || !ignoreNullValues)
            {
                writer.WriteString(ConvertName(nameof(value.Source)), value.Source);
            }

            if (value.InnerException is not null || !ignoreNullValues)
            {
                writer.WritePropertyName(ConvertName(nameof(value.InnerException)));
                if (value.InnerException is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    JsonSerializer.Serialize(writer, value.InnerException, options);
                }
            }

            writer.WriteEndObject();
        }

        private class JsonSerializedException : Exception
        {
            private readonly string _stackTrace;

            public JsonSerializedException(string type, string? message, string? source, string? stackTrace, Exception? innerException)
                : base(message, innerException)
            {
                Type = type;
                Source = source ?? string.Empty;
                _stackTrace = stackTrace ?? string.Empty;
            }

            public string Type { get; }

            public override string StackTrace => _stackTrace;
        }
    }

    private class HealthReportConverter : JsonConverter<HealthReport>
    {
        public override HealthReport Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            Dictionary<string, HealthReportEntry>? entries = null;
            TimeSpan totalDuration = TimeSpan.Zero;

            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (options.PropertyNameCaseInsensitive ? propertyName : propertyName?.ToLowerInvariant())
                    {
                        case "TotalDuration":
                        case "totalDuration":
                        case "totalduration":
                            totalDuration = JsonSerializer.Deserialize<TimeSpan>(ref reader, options);
                            break;

                        case "Entries":
                        case "entries":
                            entries = JsonSerializer.Deserialize<Dictionary<string, HealthReportEntry>>(ref reader, options);
                            break;
                    }
                }
            }

            return new HealthReport(entries ?? new(), totalDuration);
        }

        public override void Write(Utf8JsonWriter writer, HealthReport value, JsonSerializerOptions options)
        {
            string ConvertName(string name)
                => options.PropertyNamingPolicy?.ConvertName(name) ?? name;

            writer.WriteStartObject();

            writer.WritePropertyName(ConvertName(nameof(value.Status)));
            JsonSerializer.Serialize(writer, value.Status, options);

            writer.WritePropertyName(ConvertName(nameof(value.TotalDuration)));
            JsonSerializer.Serialize(writer, value.TotalDuration, options);

            writer.WritePropertyName(ConvertName(nameof(value.Entries)));
            JsonSerializer.Serialize(writer, value.Entries, options);

            writer.WriteEndObject();
        }
    }

    private class HealthReportEntryConverter : JsonConverter<HealthReportEntry>
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
}
