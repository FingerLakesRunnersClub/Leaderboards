using FLRC.Leaderboards.Core.Config;
using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Tests;

public static class TestHelpers
{
	public static AppConfig Config => JSONConfig("json/config.json");
	public static AppConfig TrailConfig => JSONConfig("json/config-trail.json");

	private static AppConfig JSONConfig(string file) => new(new ConfigurationBuilder().AddJsonFile(file).Build());
}