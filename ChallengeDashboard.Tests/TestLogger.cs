using System;
using Microsoft.Extensions.Logging;

namespace FLRC.ChallengeDashboard.Tests;

public class TestLogger : ILogger, IDisposable
{
	public IDisposable BeginScope<TState>(TState state)
	{
		return this;
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return true;
	}

	public bool Called { get; private set; }
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
	{
		Called = true;
	}

	public void Dispose()
	{
	}
}
