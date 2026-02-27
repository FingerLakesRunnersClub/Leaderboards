namespace FLRC.Leaderboards.Model;

public sealed record Series
{
	public Guid ID { get; set; }
	public string Key { get; set; } = null!;
	public string Name { get; set; } = null!;

	public ICollection<Setting> Settings { get; init; } = [];
	public ICollection<Feature> Features { get; init; } = [];
	public ICollection<Iteration> Iterations { get; init; } = [];
}