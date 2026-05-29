using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;

namespace FLRC.Leaderboards.Data.Migrations;

public sealed class LoggerUpgradeLog(ILogger log) : IUpgradeLog
{
	public void LogTrace(string format, params object[] args)
		=> log.LogTrace(format, args);

	public void LogDebug(string format, params object[] args)
		=> log.LogDebug(format, args);

	public void LogInformation(string format, params object[] args)
		=> log.LogInformation(format, args);

	public void LogWarning(string format, params object[] args)
		=> log.LogWarning(format, args);

	public void LogError(string format, params object[] args)
		=> log.LogError(format, args);

	public void LogError(Exception ex, string format, params object[] args)
		=> log.LogError(ex, format, args);
}