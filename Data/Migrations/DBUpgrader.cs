using System.Data;
using System.Reflection;
using DbUp;
using DbUp.Builder;
using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;

namespace FLRC.Leaderboards.Data.Migrations;

public sealed class DBUpgrader(IDbConnection connection, ILogger logger)
{
	private const string TableName = "_Migrations";

	private static readonly string MigrationDirectory
		= Path.Join(
			Path.GetDirectoryName(
				Assembly.GetExecutingAssembly().Location
			), "Migrations"
		);

	public void MigrateDatabase()
	{
		var upgradeLog = new LoggerUpgradeLog(logger);
		var upgrader = CreateBuilder(connection, upgradeLog)
			.WithScriptsFromFileSystem(MigrationDirectory)
			.LogTo(upgradeLog)
			.Build();

		var scriptsToExecute = upgrader.GetScriptsToExecute()
			.Select(s => s.Name)
			.ToArray();
		logger.Log(LogLevel.Information, "Starting database migrations: {scripts}", string.Join(", ", scriptsToExecute));

		var result = upgrader.PerformUpgrade();

		if (result.Successful)
			logger.Log(LogLevel.Information, "All scripts migrated successfully!");
		else
			logger.Log(LogLevel.Critical, "Database migration failed!");
	}

	private static UpgradeEngineBuilder CreateBuilder(IDbConnection connection, IUpgradeLog log)
	{
		EnsureDatabase.For.PostgresqlDatabase(connection.ConnectionString, log);
		return DeployChanges.To.PostgresqlDatabase(connection.ConnectionString)
			.JournalToPostgresqlTable("public", TableName);
	}
}