using System.Text.Json.Serialization;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Results;

public class Result : IComparable<Result>
{
	[JsonIgnore]
	public Course Course { get; init; }

	public uint? CourseID => Course?.Race.ID;
	public string CourseName => Course?.Name;
	public string CourseDistance => Course?.Distance.Display;

	public Athlete Athlete { get; init; }
	public Date StartTime { get; init; }
	public Time Duration { get; init; }

	public IDictionary<PointType, bool> CommunityPoints { get; } = new Dictionary<PointType, bool>
		{
			{ PointType.GroupRun, false },
			{ PointType.Narrative, false },
			{ PointType.LocalBusiness, false }
		};

	public byte AgeOnDayOfRun => StartTime != null
		? Athlete.AgeAsOf(StartTime.Value)
		: Athlete.Age;

	public int CompareTo(Result other) => Duration.CompareTo(other.Duration);

	public Time Behind(Result other) => Duration.Subtract(other.Duration);

	private static readonly TimeSpan GroupRunStartTimeLimit = TimeSpan.FromMinutes(5);

	public bool IsGroupRun()
		=> Course.Results.Any(r => !r.Athlete.Equals(Athlete) && r.StartTime.Value.Subtract(StartTime.Value).Duration() < GroupRunStartTimeLimit);

	public bool HasCommunityPointToday(PointType type)
		=> Course.Results.Any(r => r != this && r.Athlete.Equals(Athlete) && r.StartTime.Value.Date == StartTime.Value.Date && r.CommunityPoints[type]);
}