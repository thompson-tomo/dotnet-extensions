using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Extensions.Microsoft.Options.Tests;

public class ConfigureOptionsFromConfigurationTests
{
    private class ServiceOptions
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void ConfigureOptionsFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                [nameof(ServiceOptions.Name)] = "Service Options"
            })
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration);

        services.ConfigureOptionsFromConfiguration<ServiceOptions>();

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptions<ServiceOptions>>();

        options.Value.Name.Should().Be("Service Options");
    }

    [Fact]
    public void ConfigureOptionsFromConfigurationSection()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                [$"{nameof(ServiceOptions)}:{nameof(ServiceOptions.Name)}"] = "Service Options"
            })
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration);

        services.ConfigureOptionsFromConfiguration<ServiceOptions>(c => c.GetSection(nameof(ServiceOptions)));

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptions<ServiceOptions>>();

        options.Value.Name.Should().Be("Service Options");
    }

    [Fact]
    public void ConfigureNamedOptionsFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                [$"name:{nameof(ServiceOptions.Name)}"] = "Service Options"
            })
            .Build();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration);

        services.ConfigureOptionsFromConfiguration<ServiceOptions>("name");

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<ServiceOptions>>().Get("name");

        options.Name.Should().Be("Service Options");
    }
}