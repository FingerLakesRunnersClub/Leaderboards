namespace FLRC.Leaderboards.Data.Models;

public sealed record Iteration
{
	public required Guid ID { get; init; }
	public required Guid SeriesID { get; init; }
	public required string Name { get; init; }
	public DateOnly? StartDate { get; init; }
	public DateOnly? EndDate { get; init; }

	public required Series Series { get; init; }
	public Race[] Races { get; init; } = [];
	public Challenge[] Challenges { get; init; } = [];
}