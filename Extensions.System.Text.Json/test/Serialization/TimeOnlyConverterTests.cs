using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace Extensions.System.Text.Json.Test.Serialization;

public class TimeOnlyConverterTests
{
    [Fact]
    public void Serialize()
    {
        var value = new TimeOnly(3, 33, 33, 333);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TimeOnlyConverter("HH:mm:ss.fff"));
        var json = JsonSerializer.Serialize(value, options);

        json.Should().Be(@"""03:33:33.333""");
    }

    [Fact]
    public void Deserialize()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new TimeOnlyConverter("HH:mm:ss.fff"));
        var value = JsonSerializer.Deserialize<TimeOnly>(@"""03:33:33.333""", options);
        value.Hour.Should().Be(3);
        value.Minute.Should().Be(33);
        value.Second.Should().Be(33);
        value.Millisecond.Should().Be(333);
    }
}
