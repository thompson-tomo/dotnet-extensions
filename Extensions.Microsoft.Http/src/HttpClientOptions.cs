namespace System.Net.Http;

public abstract class HttpClientOptions<T>
    where T : HttpClient<T>
{
    public string? BaseAddress { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
    public long MaxResponseContentBufferSize { get; set; } = int.MaxValue;
}
