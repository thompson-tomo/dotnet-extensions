using System.Text.Json.Serialization;

namespace System.Text.Json;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class TimeSpanAsSecondsConverterAttribute : JsonConverterAttribute
{
    public TimeSpanAsSecondsConverterAttribute()
        : base(typeof(TimeSpanAsSecondsConverter))
    {
    }
}

public class TimeSpanAsSecondsConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => TimeSpan.FromSeconds(reader.GetDouble());

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value.TotalSeconds);
}
