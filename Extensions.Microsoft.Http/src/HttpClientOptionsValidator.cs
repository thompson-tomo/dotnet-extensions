using FluentValidation;

namespace Extensions.Microsoft.Http;

internal class HttpClientOptionsValidator<TClient, TOptions> : AbstractValidator<TOptions>
    where TOptions : HttpClientOptions<TClient>
    where TClient : HttpClient<TClient>
{
    public HttpClientOptionsValidator(string sectionName)
    {
        RuleFor(x => x.BaseAddress)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _) == true)
                .WithMessage($"{sectionName}:{nameof(HttpClientOptions<TClient>.BaseAddress)} must be a valid URI.")
                .Unless(x => string.IsNullOrEmpty(x.BaseAddress));

        RuleFor(x => x.MaxResponseContentBufferSize)
            .GreaterThan(0)
                .WithMessage($"{sectionName}:{nameof(HttpClientOptions<TClient>.MaxResponseContentBufferSize)} must be greater than zero.");
    }
}
