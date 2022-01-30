using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class OptionsServiceProviderExtensions
{
    public static TOptions GetOptions<TOptions>(this IServiceProvider serviceProvider)
        where TOptions : class
        => serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;

    public static TOptions GetOptions<TOptions>(this IServiceProvider serviceProvider, string name)
        where TOptions : class
        => serviceProvider.GetRequiredService<IOptionsSnapshot<TOptions>>().Get(name);

    public static IOptionsMonitor<TOptions> GetOptionsMonitor<TOptions>(this IServiceProvider serviceProvider)
        where TOptions : class
        => serviceProvider.GetRequiredService<IOptionsMonitor<TOptions>>();
}
