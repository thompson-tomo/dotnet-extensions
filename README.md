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

## Extensions.Microsoft.Http

[![NuGet](https://img.shields.io/nuget/dt/Extensions.Microsoft.Http.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Http)
[![NuGet](https://img.shields.io/nuget/vpre/Extensions.Microsoft.Http.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Http)

This package simplifies configuring a typed `HttpClient` using the `IOptions<T>` pattern. It will automatically bind the configuration section with the same name as the client to the options, e.g. `MicrosoftGraphClient:BaseAddress` will set the `BaseAddress` property.

```csharp
// Reads from configuration section "MicrosoftGraphClient", e.g. "MicrosoftGraphClient:BaseAddress"
services.ConfigureHttpClient<MicrosoftGraphClient, MicrosoftGraphClientOptions>();

public class MicrosoftGraphClient : HttpClient<MicrosoftGraphClient>
{
    public MicrosoftGraphClient(HttpClient client)
        : base(client)
    {
    }

    public Task<Me> GetMyAsync()
        => Client.GetFromJsonAsync<Me>("/v1.0/me"); // <-- System.Net.Http.Json package for HttpClient extensions
}

public class MicrosoftGraphClientOptions : HttpClientOptions<MicrosoftGraphClient>
{
}
```

You can also explicitly name the typed client which will change the configuration section name.

```csharp
// Reads from configuration section "NamedClient", e.g. "NamedClient:BaseAddress"
services.ConfigureHttpClient<MicrosoftGraphClient, MicrosoftGraphClientOptions>("NamedClient");
```

You can also used this for named `HttpClient` instances created with `IHttpClientFactory`.

```csharp
// Reads from configuration section "msgraph", e.g. "msgraph:BaseAddress"
services.ConfigureHttpClient<MicrosoftGraphClientOptions>("msgraph");

public class MicrosoftGraphClient
{
    private readonly HttpClient _client;

    public MicrosoftGraphClient(IHttpClientFactory factory)
        => _client = factory.CreateClient("msgraph");
}
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

Makes it easier when using `Microsoft.Extensions.Options`.

### Configuring `IOptions<T>` from `IConfiguration` 

This builds on top of the `Microsoft.Extensions.Options.ConfigurationExtensions` package.

```csharp
// Binds root IConfiguration to AppOptions
services.ConfigureOptionsFromConfiguration<AppOptions>();

// Binds configuration section "name" to the named AppOptions "name"
services.ConfigureOptionsFromConfiguration<AppOptions>("name");

// Binds configuration section "AppOptions" to AppOptions
services.ConfigureOptionsFromConfiguration<AppOptions>(c => c.GetSection(nameof(AppOptions)));

// Binds configuration section "AppOptions:name" to named AppOptions "name"
services.ConfigureOptionsFromConfiguration<AppOptions>("name", c => c.GetSection(nameof(AppOptions)));
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

## Extensions.System.Text.Json

[![NuGet](https://img.shields.io/nuget/dt/Extensions.Microsoft.Options.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Options)
[![NuGet](https://img.shields.io/nuget/vpre/Extensions.Microsoft.Options.svg)](https://www.nuget.org/packages/Extensions.Microsoft.Options)

Adds missing `System.Text.Json` converters.

### DateOnlyConverter

Adds support for serializing `DateOnly` to JSON.

```csharp
var options = new JsonSerializerOptions();
options.Converters.Add(new DateOnlyConverter("yyyy-MM-dd"));
```

### TimeOnlyConverter

Adds support for serializing `TimeOnly` to JSON.

```csharp
var options = new JsonSerializerOptions();
options.Converters.Add(new TimeOnlyConverter("HH:mm:ss.fff"));
```

### TimeSpanAsSecondsConverter

Adds support for serializing `TimeSpan` to JSON as seconds.

```csharp
public record ElapsedTime([property: TimeSpanAsSecondsConverter]TimeSpan Elapsed);
```

### DateTimeAsUnixTimeSecondsConverter

Adds support for serializing `DateTime` to JSON as seconds since the unix epoch.

```csharp
public record Document ([property: DateTimeAsUnixTimeSecondsConverter]DateTime Timestamp);
```

### DateTimeOffsetAsUnixTimeSecondsConverter

Adds support for serializing `DateTimeOffset` to JSON as seconds since the unix epoch.

```csharp
public record Document ([property: DateTimeOffsetAsUnixTimeSecondsConverter]DateTimeOffset Timestamp);
```
