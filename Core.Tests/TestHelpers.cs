using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Tests;

public static class TestHelpers
{
	public static Config Config => new(new ConfigurationBuilder().AddJsonFile("json/config.json").Build());
}