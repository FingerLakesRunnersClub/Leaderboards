namespace FLRC.Leaderboards.Model;

public sealed record Challenge : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public Guid IterationID { get; set; }
	public string Name { get; set; } = null!;
	public bool IsOfficial { get; set; }
	public bool IsPrimary { get; set; }
	public TimeSpan? TimeLimit { get; set; }

	public Iteration Iteration { get; init; } = null!;
	public ICollection<Course> Courses { get; init; } = [];

	public Athlete? Athlete { get; init; }
}