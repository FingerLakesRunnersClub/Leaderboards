using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core;

public record Config
{
	public IDictionary<uint, string> CourseNames { get; }
	public IDictionary<string, string> Links { get; }
	public FeatureSet Features { get; }

	public Config(IConfiguration config)
	{
		CourseNames = config.GetSection("Races").GetChildren().ToDictionary(c => c.GetValue<uint>("ID"), c => c.GetValue<string>("Name"));
		Links = config.GetSection("Links").GetChildren().ToDictionary(c => c.Key, c => c.Value);
		Features = new FeatureSet(config.GetSection("Features"));
	}
}