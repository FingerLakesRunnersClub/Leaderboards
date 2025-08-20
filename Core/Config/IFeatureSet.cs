namespace FLRC.Leaderboards.Core.Config;

public interface IFeatureSet
{
	bool GenerateAthleteID { get; }
	bool MultiAttempt { get; }
	bool MultiAttemptCompetitions { get; }
	bool MultiYear { get; }
	bool SelfTiming { get; }
	bool AgeGroupTeams { get; }
	bool ShowBadges { get; }
	bool CommunityStars { get; }
}