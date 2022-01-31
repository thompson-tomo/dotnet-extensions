using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace System.Text.Json;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    private readonly string? _format;
    private readonly IFormatProvider? _provider;

    public TimeOnlyConverter(string? format = null, IFormatProvider? provider = null)
    {
        _format = format;
        _provider = provider;
    }

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (_format is null)
        {
            return TimeOnly.Parse(value!, _provider);
        }

        return TimeOnly.ParseExact(value!, _format, _provider);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_format, _provider));
}
