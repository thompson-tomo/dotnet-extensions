using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Extensions.Microsoft.Logging.ApplicationInsights.Tests;

public class LoggerTelemetryTests
{
    [Fact]
    public void CanUseLogEvent()
    {
        var logger = Substitute.For<ILogger>();

        var sut = new LoggerTelemetry(logger, TelemetryClientFixture.Create());

        var result = sut.Invoking(_ => sut.LogEvent(nameof(CanUseLogEvent)));

        result.Should().NotThrow();
    }

    [Fact]
    public void CanUseLogAvailability()
    {
        var logger = Substitute.For<ILogger>();

        var sut = new LoggerTelemetry(logger, TelemetryClientFixture.Create());

        var telemetry = new Fixture()
            .Customize(new AutoNSubstituteCustomization())
            .Create<AvailabilityTelemetry>();

        var result = sut.Invoking(_ => sut.LogAvailability(telemetry));

        result.Should().NotThrow();
    }

    [Fact]
    public void CanUseLogTrace()
    {
        var logger = Substitute.For<MockLogger>();

        var sut = new LoggerTelemetry(logger, TelemetryClientFixture.Create());

        sut.LogTrace(nameof(CanUseLogTrace));

        logger.Received().Log(Arg.Any<LogLevel>(), Arg.Any<string>());
    }

    [Fact]
    public void CanUseBeginScope()
    {
        var logger = Substitute.For<MockLogger>();

        var sut = new LoggerTelemetry(logger, TelemetryClientFixture.Create());

        sut.BeginScope(this);

        logger.Received().BeginScope();
    }

    [Fact]
    public void CanUseIsEnabled()
    {
        var logger = Substitute.For<MockLogger>();

        var sut = new LoggerTelemetry(logger, TelemetryClientFixture.Create());

        sut.IsEnabled(LogLevel.Trace);

        logger.Received().IsEnabled(Arg.Is(LogLevel.Trace));
    }

    public abstract class MockLogger : ILogger
    {
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Log(logLevel, formatter(state, exception));

        public abstract void Log(LogLevel logLevel, string message);

        public virtual bool IsEnabled(LogLevel logLevel) => true;

        IDisposable ILogger.BeginScope<TState>(TState state)
            => BeginScope();

        public abstract IDisposable BeginScope();
    }
}
