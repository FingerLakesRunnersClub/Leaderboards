using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Results;

public sealed class Result : IComparable<Result>
{
	[JsonIgnore] public Course Course { get; init; }

	public uint? CourseID => Course?.Race.ID;
	public string CourseName => Course?.Name;
	public string CourseDistance => Course?.Distance.Display;

	public Athlete Athlete { get; init; }
	public Date StartTime { get; init; }
	public Time Duration { get; init; }

	public Date FinishTime => StartTime != null
		? new Date(StartTime.Value + (Duration?.Value ?? TimeSpan.Zero))
		: null;

	public IDictionary<StarType, bool> CommunityStars { get; }
		= new Dictionary<StarType, bool>
		{
			{ StarType.GroupRun, false },
			{ StarType.Story, false }
		};

	public byte AgeOnDayOfRun
		=> StartTime != null
			? Athlete.AgeAsOf(StartTime.Value)
			: Athlete.Age;

	private static readonly ConcurrentDictionary<(AgeGradeCalculator.Category, byte, double, TimeSpan), double> AgeGradeCache = new();

	[JsonIgnore]
	public double? AgeGrade
	{
		get
		{
			if (Duration is null)
				return null;
			var category = Athlete.Category?.Value ?? Category.M.Value;
			var age = AgeOnDayOfRun;
			var distance = Course.Distance.Meters;
			var duration = Duration.Value;
			var key = (category, age, distance, duration);

			return AgeGradeCache.TryGetValue(key, out var ageGrade)
				? ageGrade
				: AgeGradeCache[key] = AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(category, age, distance, duration);
		}
	}

	public int CompareTo(Result other)
		=> Duration.CompareTo(other.Duration);

	public Time Behind(Result other)
		=> Duration?.Subtract(other.Duration);

	private static readonly TimeSpan GroupRunStartTimeLimit
		= TimeSpan.FromMinutes(5);

	public bool IsGroupRun()
		=> Course.Results.Any(r => !r.Athlete.Equals(Athlete) && r.StartTime.Value.Subtract(StartTime.Value).Duration() < GroupRunStartTimeLimit);

	public bool HasCommunityStarToday(StarType type)
		=> Course.Results.Any(r => r != this && r.Athlete.Equals(Athlete) && r.StartTime.Value.Date == StartTime.Value.Date && r.CommunityStars[type]);
}