using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace Extensions.Microsoft.Diagnostics.HealthChecks.Serialization.Tests;
public class HealthReportConverterTests
{
    private readonly JsonSerializerOptions _options;

    public HealthReportConverterTests()
    {
        _options = new()
        {
            Converters =
            {
                new HealthReportConverter()
            }
        };
    }

    [Fact]
    public void Write_ShouldSerializeToJson_WhenHealthReportIsSerialized()
    {
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["entry"] = new HealthReportEntry(HealthStatus.Healthy, "description", TimeSpan.FromSeconds(1), new Exception("message"), new Dictionary<string, object>
            {
                ["Int32"] = 1,
                ["Boolean"] = true,
                ["String"] = "string"
            })
        };
        var report = new HealthReport(entries, HealthStatus.Healthy, TimeSpan.FromSeconds(1));

        var json = JsonSerializer.Serialize(report, _options);
        var document = JsonNode.Parse(json)!;

        document["Status"]!.GetValue<string>().Should().Be("Healthy");
        document["TotalDuration"]!.GetValue<string>().Should().Be("00:00:01");

        var entry = document["Entries"]!["entry"]!;

        entry["Status"]!.GetValue<string>().Should().Be("Healthy");
        entry["Duration"]!.GetValue<string>().Should().Be("00:00:01");
        entry["Data"]!["Int32"]!.GetValue<int>().Should().Be(1);
        entry["Data"]!["Boolean"]!.GetValue<bool>().Should().Be(true);
        entry["Data"]!["String"]!.GetValue<string>().Should().Be("string");
        entry["Description"]!.GetValue<string>().Should().Be("description");
        entry["Exception"]!["Message"]!.GetValue<string>().Should().Be("message");
    }

    [Fact]
    public void Read_ShouldReturnHealthReport_WhenJsonIsDeserialized()
    {
        var json = @"
{
    ""Status"": ""Healthy"",
    ""TotalDuration"":""00:00:01"",
    ""Timestamp"": ""2022-07-05T22:07:58.1595562-04:00"",
    ""Entries"": {
        ""entry"": {
            ""Status"": ""Healthy"",
            ""Description"": ""description"",
            ""Duration"": ""00:00:01"",
            ""Data"": {
                ""Int32"": 1,
                ""Boolean"": true,
                ""String"": ""string""
            },
            ""Exception"": {
                ""Type"":""System.Exception"",
                ""Message"":""message""
            },
            ""Tags"": []
        }
    }
}";

        var report = JsonSerializer.Deserialize<HealthReport>(json, _options)!;

        report.Should().NotBeNull();
        report.Status.Should().Be(HealthStatus.Healthy);
        report.TotalDuration.TotalSeconds.Should().Be(1);

        var entry = report.Entries["entry"];

        entry.Status.Should().Be(HealthStatus.Healthy);
        entry.Description.Should().Be("description");
        entry.Duration.TotalSeconds.Should().Be(1);
        entry.Data["Int32"].As<JsonElement>().GetInt32().Should().Be(1);
        entry.Data["Boolean"].As<JsonElement>().GetBoolean().Should().Be(true);
        entry.Data["String"].As<JsonElement>().GetString().Should().Be("string");
        entry.Exception!.Message.Should().Be("message");
    }
}
