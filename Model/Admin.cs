namespace FLRC.Leaderboards.Model;

public sealed record Admin
{
	public Guid ID { get; set; }

	public Athlete Athlete { get; set; } = null!;
}