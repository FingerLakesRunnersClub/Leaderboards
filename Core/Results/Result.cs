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

	public IDictionary<StarType, bool> CommunityStars { get; } = new Dictionary<StarType, bool>
		{
			{ StarType.GroupRun, false },
			{ StarType.Story, false },
			{ StarType.ShopLocal, false }
		};

	public byte AgeOnDayOfRun => StartTime != null
		? Athlete.AgeAsOf(StartTime.Value)
		: Athlete.Age;

	public int CompareTo(Result other) => Duration.CompareTo(other.Duration);

	public Time Behind(Result other) => Duration.Subtract(other.Duration);

	private static readonly TimeSpan GroupRunStartTimeLimit = TimeSpan.FromMinutes(5);

	public bool IsGroupRun()
		=> Course.Results.Any(r => !r.Athlete.Equals(Athlete) && r.StartTime.Value.Subtract(StartTime.Value).Duration() < GroupRunStartTimeLimit);

	public bool HasCommunityPointToday(StarType type)
		=> Course.Results.Any(r => r != this && r.Athlete.Equals(Athlete) && r.StartTime.Value.Date == StartTime.Value.Date && r.CommunityStars[type]);
}