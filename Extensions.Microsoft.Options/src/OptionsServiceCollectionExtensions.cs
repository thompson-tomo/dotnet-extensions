using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Extensions.Microsoft.Options;

public static class OptionsServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptionsFromConfiguration<TOptions>(this IServiceCollection services, Func<IConfiguration, IConfiguration> configurationSelector)
        where TOptions : class
        => services
            .AddOptions()
            .AddTransient<IConfigureOptions<TOptions>>(serviceProvider => new ConfigureOptions<TOptions>
                .FromConfiguration(configurationSelector(serviceProvider.GetRequiredService<IConfiguration>())));
}
