namespace FLRC.Leaderboards.Model;

public record AthleteChallenge
{
	public required Guid AthleteID { get; init; }
	public required Guid ChallengeID { get; init; }
}