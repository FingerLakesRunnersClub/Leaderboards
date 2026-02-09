namespace FLRC.Leaderboards.Data.Models;

public sealed record Result
{
	public Guid ID { get; init; }
	public Guid CourseID { get; init; }
	public Guid AthleteID { get; init; }
	public DateTime StartTime { get; init; }
	public TimeSpan Duration { get; init; }

	public Course Course { get; init; } = null!;
	public Athlete Athlete { get; init; } = null!;
}