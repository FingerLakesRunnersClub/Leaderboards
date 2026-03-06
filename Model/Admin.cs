namespace FLRC.Leaderboards.Model;

public sealed record Admin : Identifiable<Guid>
{
	public Guid ID { get; set; }

	public Athlete Athlete { get; init; } = null!;
}