namespace FLRC.Leaderboards.Model;

public record ChallengeCourse
{
	public required Guid ChallengeID { get; init; }
	public required Guid CourseID { get; init; }
}