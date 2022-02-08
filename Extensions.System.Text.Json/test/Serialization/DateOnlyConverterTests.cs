using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Xunit;

namespace Extensions.System.Text.Json.Test.Serialization;

public class DateOnlyConverterTests
{
    [Fact]
    public void Serialize()
    {
        var value = new DateOnly(2022, 1, 31);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new DateOnlyConverter("yyyy-MM-dd"));
        var json = JsonSerializer.Serialize(value, options);

        json.Should().Be(@"""2022-01-31""");
    }

    [Fact]
    public void Deserialize()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new DateOnlyConverter("yyyy-MM-dd"));
        var value = JsonSerializer.Deserialize<DateOnly>(@"""2022-01-31""", options);
        value.Year.Should().Be(2022);
        value.Month.Should().Be(1);
        value.Day.Should().Be(31);
    }
}
