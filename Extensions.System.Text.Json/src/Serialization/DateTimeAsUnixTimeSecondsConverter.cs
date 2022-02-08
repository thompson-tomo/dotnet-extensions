namespace System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class DateTimeAsUnixTimeSecondsConverterAttribute : JsonConverterAttribute
{
    public DateTimeAsUnixTimeSecondsConverterAttribute()
        : base(typeof(DateTimeAsUnixTimeSecondsConverter))
    {
    }
}

public class DateTimeAsUnixTimeSecondsConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64()).DateTime;

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteNumberValue(((DateTimeOffset)value).ToUnixTimeSeconds());
}
