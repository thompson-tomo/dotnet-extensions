namespace Microsoft.Extensions.DependencyInjection;

public static class AddDefaultRequestHeaderHttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddDefaultRequestHeader(this IHttpClientBuilder builder, string name, string? value)
        => builder.ConfigureHttpClient((serviceProvider, client) => client.DefaultRequestHeaders.Add(name, value));

    public static IHttpClientBuilder AddDefaultRequestHeader(this IHttpClientBuilder builder, string name, Func<IServiceProvider, string?> configureValue)
        => builder.ConfigureHttpClient((serviceProvider, client) =>
        {
            var value = configureValue(serviceProvider);
            client.DefaultRequestHeaders.Add(name, value);
        });

    public static IHttpClientBuilder AddDefaultRequestHeader(this IHttpClientBuilder builder, string name, IEnumerable<string?> values)
        => builder.ConfigureHttpClient((serviceProvider, client) => client.DefaultRequestHeaders.Add(name, values));

    public static IHttpClientBuilder AddDefaultRequestHeader(this IHttpClientBuilder builder, string name, Func<IServiceProvider, IEnumerable<string?>> configureValues)
        => builder.ConfigureHttpClient((serviceProvider, client) =>
        {
            var values = configureValues(serviceProvider);
            client.DefaultRequestHeaders.Add(name, values);
        });
}
