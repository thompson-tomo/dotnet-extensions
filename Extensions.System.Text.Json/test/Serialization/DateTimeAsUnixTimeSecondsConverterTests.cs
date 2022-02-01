using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Extensions.System.Text.Json.Test.Serialization;

public class DateTimeAsUnixTimeSecondsConverterTests
{
    [Fact]
    public void Serialize()
    {
        var value = new ObjectUsingDateTimeAsUnixTimeSecondsConverter { Timestamp = DateTime.Parse("2021-01-31T19:50:00Z") };
        var json = JsonSerializer.Serialize(value);
        json.Should().Be(@"{""Timestamp"":1612122600}");
    }

    [Fact]
    public void Deserialize()
    {
        var value = JsonSerializer.Deserialize<ObjectUsingDateTimeAsUnixTimeSecondsConverter>(@"{""Timestamp"":1612122600}")!.Timestamp;
        value.Year.Should().Be(2021);
        value.Month.Should().Be(1);
        value.Day.Should().Be(31);
        value.Hour.Should().Be(19);
        value.Minute.Should().Be(50);
        value.Second.Should().Be(0);
    }

    private class ObjectUsingDateTimeAsUnixTimeSecondsConverter
    {
        [DateTimeAsUnixTimeSecondsConverter]
        public DateTime Timestamp { get; set; }
    }
}
