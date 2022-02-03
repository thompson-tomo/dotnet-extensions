using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Extensions.Microsoft.Http;

internal class ConfigureHttpClient<TOptions>
    : IConfigureOptions<TOptions>,
      IConfigureNamedOptions<TOptions>,
      IValidateOptions<TOptions>
    where TOptions : HttpClientOptions
{
    private readonly IConfiguration _configuration;

    public ConfigureHttpClient(IConfiguration configuration)
        => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public void Configure(TOptions options)
        => Configure(Options.DefaultName, options);

    public void Configure(string name, TOptions options)
        => _configuration.GetSection(name).Bind(options);

    public ValidateOptionsResult Validate(string name, TOptions options)
    {
        var validator = new HttpClientOptionsValidator<TOptions>(name);
        var result = validator.Validate(options);

        if (result.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail(result.Errors.Select(x => x.ErrorMessage));
    }
}
