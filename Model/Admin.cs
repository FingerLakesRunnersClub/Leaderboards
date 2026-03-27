namespace FLRC.Leaderboards.Model;

public record Admin : Identifiable<Guid>
{
	public Guid ID { get; set; }

	public virtual Athlete Athlete { get; init; } = null!;
}