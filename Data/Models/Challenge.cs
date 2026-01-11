namespace FLRC.Leaderboards.Data.Models;

public sealed record Challenge
{
	public required Guid ID { get; init; }
	public required Guid IterationID { get; init; }
	public required string Name { get; init; }
	public required bool IsOfficial { get; init; }
	public required bool IsPublic { get; init; }
	public DateOnly? StartDate { get; init; }
	public DateOnly? EndDate { get; init; }
	public TimeSpan? TimeLimit { get; init; }

	public required Iteration Iteration { get; init; }
	public Course[] Courses { get; init; } = [];
}