using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core;

public record Config
{
	public string App { get; }
	public IDictionary<uint, string> CourseNames { get; }
	public IDictionary<string, string> Links { get; }
	public FeatureSet Features { get; }
	public IDictionary<string, string> Competitions { get; }

	public Config(IConfiguration config)
	{
		App = config.GetValue<string>("App");
		CourseNames = config.GetSection("Races").GetChildren().ToDictionary(c => c.GetValue<uint>("ID"), c => c.GetValue<string>("Name"));
		Links = GetArray(config.GetSection("Links"));
		Features = new FeatureSet(config.GetSection("Features"));
		Competitions = GetArray(config.GetSection("Competitions"));
	}

	private static IDictionary<string, string> GetArray(IConfiguration section)
		=> section.GetChildren().SelectMany(c => c.GetChildren())
			.ToDictionary(c => c.Key, c => c.Value);
}