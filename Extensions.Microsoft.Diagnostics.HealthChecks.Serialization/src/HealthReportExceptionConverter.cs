using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

internal class HealthReportExceptionConverter : JsonConverter<Exception>
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
