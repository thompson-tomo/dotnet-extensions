[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Extensions.Microsoft.Http.Tests")]

namespace System.Net.Http;

public abstract class HttpClient<T>
    where T : class
{
    protected internal HttpClient Client { get; }

    public HttpClient(HttpClient client)
        => Client = client ?? throw new ArgumentNullException(nameof(client));
}
