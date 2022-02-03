using Extensions.Microsoft.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureHttpClientServiceCollectionExtensions
{
    public static IHttpClientBuilder ConfigureHttpClient<TClient, TOptions>(this IServiceCollection services)
        where TClient : HttpClient<TClient>
        where TOptions : HttpClientOptions
    {
        services.ConfigureOptions<ConfigureHttpClient<TOptions>>();

        var builder = services.AddHttpClient<TClient>((serviceProvider, client) => ConfigureHttpClient<TOptions>(serviceProvider, client, typeof(TClient).Name));

        return builder;
    }

    public static IHttpClientBuilder ConfigureHttpClient<TClient, TOptions>(this IServiceCollection services, string name)
        where TClient : HttpClient<TClient>
        where TOptions : HttpClientOptions
    {
        services.ConfigureOptions<ConfigureHttpClient<TOptions>>();

        var builder = services.AddHttpClient<TClient>(name, (serviceProvider, client) => ConfigureHttpClient<TOptions>(serviceProvider, client, name));

        return builder;
    }

    public static IHttpClientBuilder ConfigureHttpClient<TOptions>(this IServiceCollection services, string name)
        where TOptions : HttpClientOptions
    {
        services.ConfigureOptions<ConfigureHttpClient<TOptions>>();

        var builder = services.AddHttpClient(name, (serviceProvider, client) => ConfigureHttpClient<TOptions>(serviceProvider, client, name));

        return builder;
    }

    private static void ConfigureHttpClient<TOptions>(IServiceProvider serviceProvider, HttpClient client, string name)
        where TOptions : HttpClientOptions
    {
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<TOptions>>().Get(name);

        if (options.BaseAddress is not null)
        {
            client.BaseAddress = new Uri(options.BaseAddress);
        }

        client.Timeout = options.Timeout;
        client.MaxResponseContentBufferSize = options.MaxResponseContentBufferSize;
    }
}
