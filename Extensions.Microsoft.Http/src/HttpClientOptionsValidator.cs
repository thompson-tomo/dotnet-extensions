using FluentValidation;

namespace Extensions.Microsoft.Http;

internal class HttpClientOptionsValidator<TOptions> : AbstractValidator<TOptions>
    where TOptions : HttpClientOptions
{
    public HttpClientOptionsValidator(string? name)
    {
        RuleFor(x => x.BaseAddress)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _) == true)
                .WithMessage(name is null
                    ? $"{nameof(HttpClientOptions.BaseAddress)} must be a valid URI."
                    : $"{name}:{nameof(HttpClientOptions.BaseAddress)} must be a valid URI.")
                .Unless(x => string.IsNullOrEmpty(x.BaseAddress));

        RuleFor(x => x.MaxResponseContentBufferSize)
            .GreaterThan(0)
                .WithMessage(name is null
                    ? $"{nameof(HttpClientOptions.MaxResponseContentBufferSize)} must be greater than zero."
                    : $"{name}:{nameof(HttpClientOptions.MaxResponseContentBufferSize)} must be greater than zero.");
    }
}
