using System.Text.Json.Serialization;

namespace FLRC.ChallengeDashboard;

public class Result : IComparable<Result>
{
	[JsonIgnore]
	public Course Course { get; init; }
	public uint? CourseID => Course?.ID;
	public string CourseName => Course?.Name;

	public Athlete Athlete { get; init; }
	public Date StartTime { get; init; }
	public Time Duration { get; init; }

	public byte AgeOnDayOfRun => StartTime != null
		? Athlete.AgeAsOf(StartTime.Value)
		: Athlete.Age;

	public int CompareTo(Result other) => Duration.CompareTo(other.Duration);

	public Time Behind(Result other) => Duration.Subtract(other.Duration);
}