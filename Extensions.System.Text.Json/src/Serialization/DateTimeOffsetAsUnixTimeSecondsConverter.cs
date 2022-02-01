using System.Text.Json.Serialization;

namespace System.Text.Json;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class DateTimeOffsetAsUnixTimeSecondsConverterAttribute : JsonConverterAttribute
{
    public DateTimeOffsetAsUnixTimeSecondsConverterAttribute()
        : base(typeof(DateTimeOffsetAsUnixTimeSecondsConverter))
    {
    }
}

public class DateTimeOffsetAsUnixTimeSecondsConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64());

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value.ToUnixTimeSeconds());
}
