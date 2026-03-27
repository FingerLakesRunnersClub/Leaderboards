using System.Text.Json.Serialization;

namespace FLRC.Leaderboards.Model;

public record Race : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public string Name { get; set; } = null!;
	public string Type { get; set; } = null!;
	public string Description { get; set; } = null!;

	[JsonIgnore]
	public virtual ICollection<Iteration> Iterations { get; init; } = [];

	[JsonIgnore]
	public virtual ICollection<Course> Courses { get; init; } = [];

	[JsonIgnore]
	public virtual ICollection<RaceLink> Links { get; init; } = [];

	public string DistanceDisplay => string.Join(", ", Courses.Select(c => c.DistanceDisplay));
}