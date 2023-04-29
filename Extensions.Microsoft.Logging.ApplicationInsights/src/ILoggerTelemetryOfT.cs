namespace Microsoft.Extensions.Logging;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Marker for the category name")]
public interface ILoggerTelemetry<out T> : ILoggerTelemetry
{
}
