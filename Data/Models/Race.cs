namespace FLRC.Leaderboards.Data.Models;

public sealed record Race
{
	public Guid ID { get; set; }
	public string Name { get; set; } = null!;
	public string Type { get; set; } = null!;

	public ICollection<Iteration> Iterations { get; init; } = [];
	public ICollection<Course> Courses { get; init; } = [];

	public string Distances => string.Join(", ", Courses.Select(c => $"{c.Distance:#0.##} {c.Units}"));
}