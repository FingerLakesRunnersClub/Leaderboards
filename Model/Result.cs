namespace FLRC.Leaderboards.Model;

public record Result : Identifiable<Guid>
{
	public Guid ID { get; set; }
	public Guid CourseID { get; set; }
	public Guid AthleteID { get; set; }
	public DateTime StartTime { get; set; }
	public TimeSpan Duration { get; set; }

	public virtual Course Course { get; init; } = null!;
	public virtual Athlete Athlete { get; init; } = null!;

	public byte? AthleteAge
		=> Athlete.AgeAsOf(StartTime);

	public DateTime FinishTime => StartTime + Duration;
}