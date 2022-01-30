namespace Microsoft.Extensions.Diagnostics.HealthChecks.ApplicationInsights;

public class ApplicationInsightsHealthCheckPublisherOptions
{
    public string RunLocation { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public string EnvironmentName { get; set; } = string.Empty;
}
