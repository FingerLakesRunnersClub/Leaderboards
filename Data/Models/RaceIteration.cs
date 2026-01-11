namespace FLRC.Leaderboards.Data.Models;

public sealed record RaceIteration
{
	public required Guid IterationID { get; init; }
	public required Guid RaceID { get; init; }
}