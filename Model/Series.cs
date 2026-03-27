using System.Text.Json.Serialization;

namespace FLRC.Leaderboards.Model;

public record Series : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public string Key { get; set; } = null!;
	public string Name { get; set; } = null!;

	[JsonIgnore]
	public virtual ICollection<Setting> Settings { get; init; } = [];

	[JsonIgnore]
	public virtual ICollection<Feature> Features { get; init; } = [];

	[JsonIgnore]
	public virtual ICollection<Iteration> Iterations { get; init; } = [];
}