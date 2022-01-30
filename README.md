# It's dangerous to go alone! Take these...

[![Continuous Integration](https://github.com/smokedlinq/dotnet-extensions/actions/workflows/ci.yml/badge.svg)](https://github.com/smokedlinq/dotnet-extensions/actions/workflows/ci.yml)
[![Publish](https://github.com/smokedlinq/dotnet-extensions/actions/workflows/publish.yml/badge.svg)](https://github.com/smokedlinq/dotnet-extensions/actions/workflows/publish.yml)

## Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights

[![NuGet](https://img.shields.io/nuget/dt/Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights)
[![NuGet](https://img.shields.io/nuget/vpre/Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights)

## Extensions.Microsoft.Logging.ApplicationInsights

[![NuGet](https://img.shields.io/nuget/dt/Extensions.Microsoft.Logging.ApplicationInsights.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Logging.ApplicationInsights)
[![NuGet](https://img.shields.io/nuget/vpre/Extensions.Microsoft.Logging.ApplicationInsights.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Logging.ApplicationInsights)

This package adds support for logging to both Application Insights and Microsoft.Extensions.Logging from a common interface thus removing the need for a dependency on `ILogger` and `TelemetryClient`.

### Getting started

First, configure dependency injection to include the `ILoggerTelemetry<T>` and `ILoggerTelemetryFactory` interfaces.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add logging and Application Insights if your platform does not
    // services.AddLogging();
    // serivces.AddApplicationInsightsTelemetry();
    services.AddLoggerTelemetry()
}
```

Then use one of the following to take a dependency:

```csharp
private readonly ILoggerTelemetry logger;

public MyClass(ILoggerTelemetry<MyClass> logger)
    => this.logger = logger;

public MyClass(ILoggerTelemetryFactory loggerFactory)
    => this.logger = loggerFactory.CreateLogger<MyClass>();
```

Finally, use the class to log like you would with `ILogger` and also be able to pass availability and event telemetry to the `TelemetryClient`.

```csharp
public void ChangeTheThing()
{
   logger.LogTrace("About to change the thing");
   // ...
   logger.LogEvent(nameof(ChangeTheThing));
}
```

> Note: Only requests to the ILoggerTelemetry methods `LogAvailability` and `LogEvent` are directly passed to the TelemetryClient, all other logging is passed to the `ILogger`. The ILogger needs to be configured to use the Application Insights provider to forward other telemetry.
