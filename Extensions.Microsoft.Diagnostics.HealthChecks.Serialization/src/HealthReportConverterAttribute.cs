using System.Text.Json.Serialization;

namespace Microsoft.Extensions.Diagnostics.HealthChecks;

public class HealthReportConverterAttribute : JsonConverterAttribute
{
    public HealthReportConverterAttribute()
        : base(typeof(HealthReportConverter))
    {
    }
}
