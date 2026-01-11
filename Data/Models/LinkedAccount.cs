namespace FLRC.Leaderboards.Data.Models;

public sealed record LinkedAccount
{
	public required Guid ID { get; init; }
	public required Guid AthleteID { get; init; }
	public required string Type { get; init; }
	public required string Value { get; init; }

	public required Athlete Athlete { get; init; }
}