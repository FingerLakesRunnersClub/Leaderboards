using FLRC.Leaderboards.Core.Config;
using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Tests;

public static class TestHelpers
{
	public static IConfig Config => new AppConfig(new ConfigurationBuilder().AddJsonFile("json/config.json").Build());
}