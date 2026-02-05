using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;

namespace FLRC.Leaderboards.Data.Migrations;

public sealed class LoggerUpgradeLog : IUpgradeLog
{
	private readonly ILogger _log;

	public LoggerUpgradeLog(ILogger log)
		=> _log = log;

	public void LogTrace(string format, params object[] args)
		=> _log.LogTrace(format, args);

	public void LogDebug(string format, params object[] args)
		=> _log.LogDebug(format, args);

	public void LogInformation(string format, params object[] args)
		=> _log.LogInformation(format, args);

	public void LogWarning(string format, params object[] args)
		=> _log.LogWarning(format, args);

	public void LogError(string format, params object[] args)
		=> _log.LogError(format, args);

	public void LogError(Exception ex, string format, params object[] args)
		=> _log.LogError(ex, format, args);
}