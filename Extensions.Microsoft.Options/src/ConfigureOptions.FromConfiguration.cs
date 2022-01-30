using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Extensions.Microsoft.Options;

public static partial class ConfigureOptions<TOptions>
    where TOptions : class
{
    public class FromConfiguration : IConfigureOptions<TOptions>
    {
        private readonly IConfiguration _configuration;

        public FromConfiguration(IConfiguration configuration)
            => _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        public void Configure(TOptions options)
            => _configuration.Bind(options);
    }
}
