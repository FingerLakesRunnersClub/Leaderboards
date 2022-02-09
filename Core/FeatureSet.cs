using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core;

public record FeatureSet
{
	public bool GenerateAthleteID { get; }
	public bool MultiAttempt { get; }
	public bool MultiDistance { get; }

	public FeatureSet(IConfiguration section)
	{
		GenerateAthleteID = section.GetValue<bool>(nameof(GenerateAthleteID));
		MultiAttempt = section.GetValue<bool>(nameof(MultiAttempt));
		MultiDistance = section.GetValue<bool>(nameof(MultiDistance));
	}
}