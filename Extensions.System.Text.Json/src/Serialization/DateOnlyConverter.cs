using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace System.Text.Json;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private readonly string? _format;
    private readonly IFormatProvider? _provider;

    public DateOnlyConverter(string? format = null, IFormatProvider? provider = null)
    {
        _format = format;
        _provider = provider;
    }

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (_format is null)
        {
            return DateOnly.Parse(value!, _provider);
        }

        return DateOnly.ParseExact(value!, _format, _provider);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_format, _provider));
}
