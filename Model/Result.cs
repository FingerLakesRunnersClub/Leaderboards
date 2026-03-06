namespace FLRC.Leaderboards.Model;

public sealed record Result : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public Guid CourseID { get; set; }
	public Guid AthleteID { get; set; }
	public DateTime StartTime { get; set; }
	public TimeSpan Duration { get; set; }

	public Course Course { get; init; } = null!;
	public Athlete Athlete { get; init; } = null!;
}