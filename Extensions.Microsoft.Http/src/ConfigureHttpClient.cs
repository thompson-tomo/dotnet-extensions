using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Extensions.Microsoft.Http;

internal class ConfigureHttpClient<TClient, TOptions>
    : IConfigureOptions<TOptions>,
      IValidateOptions<TOptions>
    where TOptions : HttpClientOptions<TClient>
    where TClient : HttpClient<TClient>
{
    private static readonly string SectionName = typeof(TClient).Name;
    private static readonly HttpClientOptionsValidator<TClient, TOptions> Validator = new(SectionName);

    private readonly IConfiguration _configuration;

    public ConfigureHttpClient(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void Configure(TOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }

    public ValidateOptionsResult Validate(string name, TOptions options)
    {
        var result = Validator.Validate(options);

        if (result.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail(result.Errors.Select(x => x.ErrorMessage));
    }
}
