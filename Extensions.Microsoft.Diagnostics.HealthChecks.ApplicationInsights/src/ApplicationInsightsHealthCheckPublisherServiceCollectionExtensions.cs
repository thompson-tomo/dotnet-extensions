using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks.ApplicationInsights;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationInsightsHealthCheckPublisherServiceCollectionExtensions
{
    public static IHealthChecksBuilder AddApplicationInsightsPublisher(this IHealthChecksBuilder builder, Action<ApplicationInsightsHealthCheckPublisherOptions>? configure = null)
    {
        _ = builder ?? throw new ArgumentNullException(nameof(builder));
        builder.Services.AddOptions<ApplicationInsightsHealthCheckPublisherOptions>()
            .Configure(options =>
            {
                var websiteSiteName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME");
                var websiteSlotName = Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME");

                options.RunLocation = string.IsNullOrEmpty(websiteSiteName) ? Environment.MachineName : websiteSiteName;
                options.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
                options.EnvironmentName = string.IsNullOrEmpty(websiteSlotName) ? Environment.MachineName : websiteSlotName;
            })
            .Configure<IConfiguration>((options, configuration) => configuration.GetSection(nameof(ApplicationInsightsHealthCheckPublisher)).Bind(options))
            .Configure(options => configure?.Invoke(options));
        builder.Services.AddSingleton<IHealthCheckPublisher, ApplicationInsightsHealthCheckPublisher>();
        return builder;
    }
}
