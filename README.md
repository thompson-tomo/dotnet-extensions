# It's dangerous to go alone! Take these...

[![Continuous Integration](https://github.com/smokedlinq/dotnet-extensions/actions/workflows/ci.yml/badge.svg)](https://github.com/smokedlinq/dotnet-extensions/actions/workflows/ci.yml)
[![Publish](https://github.com/smokedlinq/dotnet-extensions/actions/workflows/publish.yml/badge.svg)](https://github.com/smokedlinq/dotnet-extensions/actions/workflows/publish.yml)

## Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights

[![NuGet](https://img.shields.io/nuget/dt/Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights)
[![NuGet](https://img.shields.io/nuget/vpre/Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Diagnostics.HealthChecks.ApplicationInsights)

This package adds an `IHealthCheckPublisher` to `Microsoft.Diagnostics.HealthChecks` that publishes health data as availability telemetry to Application Insights.

```csharp
services
    .AddHealthChecks()
    .AddApplicationInsightsPublisher();
```

## Extensions.Microsoft.Logging.ApplicationInsights

[![NuGet](https://img.shields.io/nuget/dt/Extensions.Microsoft.Logging.ApplicationInsights.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Logging.ApplicationInsights)
[![NuGet](https://img.shields.io/nuget/vpre/Extensions.Microsoft.Logging.ApplicationInsights.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Logging.ApplicationInsights)

This package adds support for logging to both Application Insights and Microsoft.Extensions.Logging from a common interface thus removing the need for a dependency on `ILogger` and `TelemetryClient`.

Configure dependency injection to include the `ILoggerTelemetry<T>` and `ILoggerTelemetryFactory` interfaces.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add logging and Application Insights if your platform does not
    // services.AddLogging();
    // serivces.AddApplicationInsightsTelemetry();
    services.AddLoggerTelemetry()
}
```

Use one of the following to take a dependency:

```csharp
private readonly ILoggerTelemetry logger;

public MyClass(ILoggerTelemetry<MyClass> logger)
    => this.logger = logger;

public MyClass(ILoggerTelemetryFactory loggerFactory)
    => this.logger = loggerFactory.CreateLogger<MyClass>();
```

Use the class to log like you would with `ILogger` and also be able to pass availability and event telemetry to the `TelemetryClient`.

```csharp
public void ChangeTheThing()
{
   logger.LogTrace("About to change the thing");
   // ...
   logger.LogEvent(nameof(ChangeTheThing));
}
```

*Note: Requests to the ILoggerTelemetry methods `LogAvailability` and `LogEvent` are directly passed to the TelemetryClient, all other logging to the `ILogger`.*

## Extensions.Microsoft.Options

[![NuGet](https://img.shields.io/nuget/dt/Extensions.Microsoft.Options.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Options)
[![NuGet](https://img.shields.io/nuget/vpre/Extensions.Microsoft.Options.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Options)

This package makes it easier using `Microsoft.Extensions.Options` easier.

### Configure options from `IConfiguration` data

```csharp
services.ConfigureOptionsFromConfiguration<AppOptions>(configuration => configuration.GetSection("App"));
```

You can also use the `ConfigureOptions` pattern to keep your configuration logic in a separate class.

```csharp
public class AppOptionsFromConfiguration : ConfigureOptions<AppOptions>.FromConfiguration
{
    public AppOptionsFromConfiguration(IConfiguration configuration)
        : base(configuration.GetSection("App"))
    { }
}

services.ConfigureOptions<AppOptionsFromConfiguration>();
```

### Write less code when using `IServiceProvider` to get options

```csharp
// serviceProvider.GetRequiredService<IOptions<AppOptions>>().Value;
serviceProvider.GetOptions<AppOptions>();

// serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Get("name");
serviceProvider.GetOptions<AppOptions>("name");

// serviceProvider.GetRequiredService<IOptionsMonitor<AppOptions>>();
serviceProvider.GetOptionsMonitor<AppOptions>();
```
