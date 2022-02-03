namespace System.Net.Http;

public abstract class HttpClientOptions
{
    public string? BaseAddress { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
    public long MaxResponseContentBufferSize { get; set; } = int.MaxValue;
}
