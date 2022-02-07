using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core;

public record FeatureSet
{
	public bool MultiAttempt { get; }
	public bool MultiDistance { get; }

	public FeatureSet(IConfiguration section)
	{
		MultiAttempt = section.GetValue<bool>("MultiAttempt");
		MultiDistance = section.GetValue<bool>("MultiDistance");
	}
}