namespace FLRC.Leaderboards.Data.Models;

public sealed record Race
{
	public required Guid ID { get; init; }
	public required string Name { get; init; }
	public required string Type { get; init; }

	public Iteration[] Iterations { get; init; } = [];
	public Course[] Courses { get; init; } = [];
}