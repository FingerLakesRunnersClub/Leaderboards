using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Config;

public record FeatureSet : IFeatureSet
{
	public bool GenerateAthleteID { get; }
	public bool MultiAttempt { get; }
	public bool AgeGroupTeams { get; }
	public bool ShowBadges { get; }
	public bool CommunityStars { get; }
	public bool Monthly { get; }

	public FeatureSet(IConfiguration section)
	{
		GenerateAthleteID = section.GetValue<bool>(nameof(GenerateAthleteID));
		MultiAttempt = section.GetValue<bool>(nameof(MultiAttempt));
		AgeGroupTeams = section.GetValue<bool>(nameof(AgeGroupTeams));
		ShowBadges = section.GetValue<bool>(nameof(ShowBadges));
		CommunityStars = section.GetValue<bool>(nameof(CommunityStars));
		Monthly = section.GetValue<bool>(nameof(Monthly));
	}
}