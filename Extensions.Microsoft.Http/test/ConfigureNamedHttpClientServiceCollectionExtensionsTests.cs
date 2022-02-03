using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Extensions.Microsoft.Http.Tests;

public class ConfigureNamedHttpClientServiceCollectionExtensionsTests
{
    [Fact]
    public void ConfigureHttpClient()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        services.ConfigureHttpClient<TestClientOptions>("name");

        using var serviceProvider = services.BuildServiceProvider();

        var client = serviceProvider.GetService<IHttpClientFactory>()?
            .CreateClient("name");

        client.Should().NotBeNull();
    }

    [Fact]
    public void ConfigureHttpClientWithBaseAddress()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["name:BaseAddress"] = "http://localhost"
            })
            .Build());

        services.ConfigureHttpClient<TestClientOptions>("name");

        using var serviceProvider = services.BuildServiceProvider();

        var client = serviceProvider.GetRequiredService<IHttpClientFactory>()
            .CreateClient("name");

        client.BaseAddress?.Host.Should().Be("localhost");
    }

    [Fact]
    public void ConfigureHttpClientWithTimeout()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["name:Timeout"] = "00:00:30"
            })
            .Build());

        services.ConfigureHttpClient<TestClientOptions>("name");

        using var serviceProvider = services.BuildServiceProvider();

        var client = serviceProvider.GetRequiredService<IHttpClientFactory>()
            .CreateClient("name");

        client.Timeout.TotalSeconds.Should().Be(30);
    }

    private class TestClientOptions : HttpClientOptions
    {
    }
}
