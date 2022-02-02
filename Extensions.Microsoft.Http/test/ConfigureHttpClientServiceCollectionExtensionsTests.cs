using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Extensions.Microsoft.Http.Tests;

public class ConfigureHttpClientServiceCollectionExtensionsTests
{
    [Fact]
    public void ConfigureHttpClient()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        services.ConfigureHttpClient<TestClient, TestClientOptions>();

        using var serviceProvider = services.BuildServiceProvider();

        var client = serviceProvider.GetService<TestClient>();

        client.Should().NotBeNull();
    }

    [Fact]
    public void ConfigureHttpClientWithBaseAddress()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TestClient:BaseAddress"] = "http://localhost"
            })
            .Build());

        services.ConfigureHttpClient<TestClient, TestClientOptions>();

        using var serviceProvider = services.BuildServiceProvider();

        var client = serviceProvider.GetRequiredService<TestClient>();

        client.Client.BaseAddress?.Host.Should().Be("localhost");
    }

    [Fact]
    public void ConfigureHttpClientWithTimeout()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["TestClient:Timeout"] = "00:00:30"
            })
            .Build());

        services.ConfigureHttpClient<TestClient, TestClientOptions>();

        using var serviceProvider = services.BuildServiceProvider();

        var client = serviceProvider.GetRequiredService<TestClient>();

        client.Client.Timeout.TotalSeconds.Should().Be(30);
    }

    private class TestClient : HttpClient<TestClient>
    {
        public TestClient(HttpClient client)
            : base(client)
        {
        }
    }

    private class TestClientOptions : HttpClientOptions<TestClient>
    {
    }
}
