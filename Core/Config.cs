using System.Collections.Specialized;
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
		Links = config.GetSection("Links").GetChildren().ToDictionary(c => c.Key, c => c.Value);
		Features = new FeatureSet(config.GetSection("Features"));
		Competitions = config.GetSection("Competitions").GetChildren().SelectMany(c => c.GetChildren()).ToDictionary(c => c.Key, c => c.Value);
	}
}