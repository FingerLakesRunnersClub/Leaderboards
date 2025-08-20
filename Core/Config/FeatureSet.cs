using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Config;

public sealed record FeatureSet : IFeatureSet
{
	public bool GenerateAthleteID { get; }
	public bool MultiAttempt { get; }
	public bool MultiAttemptCompetitions { get; }
	public bool MultiYear { get; }
	public bool SelfTiming { get; }
	public bool AgeGroupTeams { get; }
	public bool ShowBadges { get; }
	public bool CommunityStars { get; }
	public bool FileSystemResults { get; }

	public FeatureSet(IConfiguration section)
	{
		GenerateAthleteID = section.GetValue<bool>(nameof(GenerateAthleteID));
		MultiAttempt = section.GetValue<bool>(nameof(MultiAttempt));
		MultiAttemptCompetitions = section.GetValue<bool>(nameof(MultiAttemptCompetitions));
		MultiYear = section.GetValue<bool>(nameof(MultiYear));
		SelfTiming = section.GetValue<bool>(nameof(SelfTiming));
		AgeGroupTeams = section.GetValue<bool>(nameof(AgeGroupTeams));
		ShowBadges = section.GetValue<bool>(nameof(ShowBadges));
		CommunityStars = section.GetValue<bool>(nameof(CommunityStars));
		FileSystemResults = section.GetValue<bool>(nameof(FileSystemResults));
	}
}