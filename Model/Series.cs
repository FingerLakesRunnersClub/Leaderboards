namespace FLRC.Leaderboards.Model;

public record Series : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public string Key { get; set; } = null!;
	public string Name { get; set; } = null!;

	public virtual ICollection<Setting> Settings { get; init; } = [];
	public virtual ICollection<Feature> Features { get; init; } = [];
	public virtual ICollection<Iteration> Iterations { get; init; } = [];
}