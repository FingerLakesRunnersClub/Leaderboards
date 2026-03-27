namespace FLRC.Leaderboards.Model;

public record RaceIteration
{
	public required Guid IterationID { get; init; }
	public required Guid RaceID { get; init; }
}