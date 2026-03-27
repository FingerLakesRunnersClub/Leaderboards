namespace FLRC.Leaderboards.Model;

public record IterationRegistration
{
	public required Guid IterationID { get; init; }
	public required Guid AthleteID { get; init; }
}