using System.ComponentModel.DataAnnotations;

namespace System.Net.Http;

public abstract class HttpClientOptions
{
    [Url()]
    public string? BaseAddress { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
    [Range(1, long.MaxValue)]
    public long MaxResponseContentBufferSize { get; set; } = int.MaxValue;
}
