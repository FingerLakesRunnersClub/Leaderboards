namespace FLRC.Leaderboards.Data.Models;

public sealed record Result
{
	public required Guid ID { get; init; }
	public required Guid CourseID { get; init; }
	public required Guid AthleteID { get; init; }
	public required DateTime StartTime { get; init; }
	public required int Duration { get; init; }

	public required Course Course { get; init; }
	public required Athlete Athlete { get; init; }
}