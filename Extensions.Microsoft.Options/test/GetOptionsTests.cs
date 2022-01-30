using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Extensions.Microsoft.Options.Tests;

public class GetOptionsTests
{
    private class ServiceOptions
    {
    }

    [Fact]
    public void GetOptions()
    {
        var services = new ServiceCollection();

        services.AddOptions<ServiceOptions>();

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetOptions<ServiceOptions>();

        options.Should().NotBeNull();
    }

    [Fact]
    public void GetNamedOptions()
    {
        var services = new ServiceCollection();

        services.AddOptions<ServiceOptions>("name");

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetOptions<ServiceOptions>("name");

        options.Should().NotBeNull();
    }

    [Fact]
    public void GetOptionsMonitor()
    {
        var services = new ServiceCollection();

        services.AddOptions<ServiceOptions>();

        using var serviceProvider = services.BuildServiceProvider();

        var options = serviceProvider.GetOptionsMonitor<ServiceOptions>();

        options.CurrentValue.Should().NotBeNull();
    }
}