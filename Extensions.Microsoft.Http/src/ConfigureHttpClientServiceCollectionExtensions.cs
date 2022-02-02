using Extensions.Microsoft.Http;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureHttpClientServiceCollectionExtensions
{
    public static IHttpClientBuilder ConfigureHttpClient<TClient, TOptions>(this IServiceCollection services)
        where TClient : HttpClient<TClient>
        where TOptions : HttpClientOptions<TClient>
    {
        services.ConfigureOptions<ConfigureHttpClient<TClient, TOptions>>();

        var builder = services
            .AddHttpClient<TClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;

                if (options.BaseAddress is not null)
                {
                    client.BaseAddress = new Uri(options.BaseAddress);
                }

                client.Timeout = options.Timeout;
                client.MaxResponseContentBufferSize = options.MaxResponseContentBufferSize;
            });

        return builder;
    }
}
