using System.Data;
using System.Reflection;
using DbUp;
using DbUp.Builder;
using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;

namespace FLRC.Leaderboards.Data.Migrations;

public sealed class DBUpgrader
{
	private const string TableName = "_Migrations";

	private static readonly string MigrationDirectory
		= Path.Join(
			Path.GetDirectoryName(
				Assembly.GetExecutingAssembly().Location
			), "Migrations"
		);

	private readonly IDbConnection _connection;
	private readonly ILogger _logger;

	public DBUpgrader(IDbConnection connection, ILogger logger)
	{
		_connection = connection;
		_logger = logger;
	}

	public void MigrateDatabase()
	{
		var upgradeLog = new LoggerUpgradeLog(_logger);
		var upgrader = CreateBuilder(_connection, upgradeLog)
			.WithScriptsFromFileSystem(MigrationDirectory)
			.LogTo(upgradeLog)
			.Build();

		var scriptsToExecute = upgrader.GetScriptsToExecute()
			.Select(s => s.Name)
			.ToArray();
		_logger.Log(LogLevel.Information, "Starting database migrations: {scripts}", string.Join(", ", scriptsToExecute));

		var result = upgrader.PerformUpgrade();

		if (result.Successful)
			_logger.Log(LogLevel.Information, "All scripts migrated successfully!");
		else
			_logger.Log(LogLevel.Critical, "Database migration failed!");
	}

	private static UpgradeEngineBuilder CreateBuilder(IDbConnection connection, IUpgradeLog log)
	{
		EnsureDatabase.For.PostgresqlDatabase(connection.ConnectionString, log);
		return DeployChanges.To.PostgresqlDatabase(connection.ConnectionString)
			.JournalToPostgresqlTable("public", TableName);
	}
}