namespace FLRC.Leaderboards.Core.Config;

public interface IFeatureSet
{
	bool GenerateAthleteID { get; }
	bool MultiAttempt { get; }
	bool AgeGroupTeams { get; }
	bool ShowBadges { get; }
	bool CommunityPoints { get; }
}