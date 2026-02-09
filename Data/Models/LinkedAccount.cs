namespace FLRC.Leaderboards.Data.Models;

public sealed record LinkedAccount
{
	public Guid ID { get; init; }
	public Guid AthleteID { get; init; }
	public string Type { get; init; } = null!;
	public string Value { get; init; } = null!;

	public Athlete Athlete { get; init; } = null!;
}