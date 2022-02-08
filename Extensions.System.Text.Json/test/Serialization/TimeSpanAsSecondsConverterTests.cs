using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Xunit;

namespace Extensions.System.Text.Json.Test.Serialization;

public class TimeSpanAsSecondsConverterTests
{
    [Fact]
    public void Serialize()
    {
        var value = new ObjectUsingTimeSpanAsSecondsConverter { Timestamp = TimeSpan.FromSeconds(5.5) };
        var json = JsonSerializer.Serialize(value);
        var sut = JsonDocument.Parse(json);
        var element = sut.RootElement.GetProperty(nameof(ObjectUsingTimeSpanAsSecondsConverter.Timestamp));
        var propertyValue = element.GetDouble();

        propertyValue.Should().Be(5.5);
    }

    [Fact]
    public void Deserialize()
    {
        var value = JsonSerializer.Deserialize<ObjectUsingTimeSpanAsSecondsConverter>(@"{""Timestamp"":5.5}");
        value!.Timestamp.TotalSeconds.Should().Be(5.5);
    }

    private class ObjectUsingTimeSpanAsSecondsConverter
    {
        [TimeSpanAsSecondsConverter]
        public TimeSpan Timestamp { get; set; }
    }
}
