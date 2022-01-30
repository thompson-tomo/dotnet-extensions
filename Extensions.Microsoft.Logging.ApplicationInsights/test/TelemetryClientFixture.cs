using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Extensions.Microsoft.Logging.ApplicationInsights.Tests;

internal static class TelemetryClientFixture
{
    public static TelemetryClient Create()
    {
        var configuration = TelemetryConfiguration.CreateDefault();
        configuration.DisableTelemetry = true;
        return new TelemetryClient(configuration);
    }
}
