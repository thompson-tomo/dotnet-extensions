using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Options;

public static class OptionsServiceCollectionExtensions
{
    public static IServiceCollection ConfigureOptionsFromConfiguration<TOptions>(this IServiceCollection services, Func<IConfiguration, IConfiguration>? configurationSelector = null)
        where TOptions : class
    {
        var selector = (IServiceProvider serviceProvider) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            if (configurationSelector is null)
            {
                return configuration;
            }

            return configurationSelector(configuration);
        };

        services.AddOptions();

        services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(serviceProvider =>
            new ConfigurationChangeTokenSource<TOptions>(selector(serviceProvider)));

        services.AddSingleton<IConfigureOptions<TOptions>>(serviceProvider =>
            new ConfigureFromConfigurationOptions<TOptions>(selector(serviceProvider)));

        return services;
    }

    public static IServiceCollection ConfigureOptionsFromConfiguration<TOptions>(this IServiceCollection services, string name, Func<IConfiguration, IConfiguration>? configurationSelector = null)
        where TOptions : class
    {
        var selector = (IServiceProvider serviceProvider) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            if (configurationSelector is null)
            {
                return configuration.GetSection(name);
            }

            return configurationSelector(configuration).GetSection(name);
        };

        services.AddOptions();

        services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(serviceProvider =>
            new ConfigurationChangeTokenSource<TOptions>(name, selector(serviceProvider)));

        services.AddSingleton<IConfigureOptions<TOptions>>(serviceProvider =>
            new NamedConfigureFromConfigurationOptions<TOptions>(name, selector(serviceProvider), _ => { }));

        return services;
    }
}
