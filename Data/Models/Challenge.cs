namespace FLRC.Leaderboards.Data.Models;

public sealed record Challenge
{
	public Guid ID { get; set; }
	public Guid IterationID { get; set; }
	public string Name { get; set; } = null!;
	public bool IsOfficial { get; set; }
	public bool IsPublic { get; set; }
	public DateOnly? StartDate { get; set; }
	public DateOnly? EndDate { get; set; }
	public TimeSpan? TimeLimit { get; set; }

	public Iteration Iteration { get; init; } = null!;
	public Course[] Courses { get; init; } = [];
}