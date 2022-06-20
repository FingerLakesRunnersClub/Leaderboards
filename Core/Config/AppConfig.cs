using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Config;

public record AppConfig : IConfig
{
	public string App { get; }
	public IFeatureSet Features { get; }

	public IDictionary<uint, string> CourseNames { get; }
	public IDictionary<string, string> Links { get; }
	public IDictionary<string, string> Competitions { get; }
	public IDictionary<string, byte> Awards { get; }

	public string AliasAPI { get; }
	public string GroupAPI { get; }

	public string CommunityURL { get; }
	public string CommunityKey { get; }


	public AppConfig(IConfiguration config)
	{
		App = config.GetValue<string>("App");
		Features = new FeatureSet(config.GetSection("Features"));
		CourseNames = config.GetSection("Races").GetChildren().ToDictionary(c => c.GetValue<uint>("ID"), c => c.GetValue<string>("Name"));
		Links = GetStringDictionary(config.GetSection("Links"));
		Competitions = GetStringDictionary(config.GetSection("Competitions"));
		Awards = GetByteDictionary(config.GetSection("Awards"));

		AliasAPI = config.GetValue<string>("AliasAPI");
		GroupAPI = config.GetValue<string>("GroupAPI");

		CommunityURL = config.GetValue<string>("CommunityURL");
		CommunityKey = Environment.GetEnvironmentVariable("CommunityKey");
	}

	private static IDictionary<string, string> GetStringDictionary(IConfiguration section)
		=> section.GetChildren().SelectMany(c => c.GetChildren())
			.ToDictionary(c => c.Key, c => c.Value);

	private static IDictionary<string, byte> GetByteDictionary(IConfiguration section)
		=> section.GetChildren().SelectMany(c => c.GetChildren())
			.ToDictionary(c => c.Key, c => byte.Parse(c.Value));
}