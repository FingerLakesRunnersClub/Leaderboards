using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using FLRC.AgeGradeCalculator;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using Category = FLRC.Leaderboards.Core.Athletes.Category;

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
	public Distance Distance { get; init; }

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

	private static readonly ConcurrentDictionary<(AgeGradeCalculator.Category, byte, double, TimeSpan), double> RoadAgeGradeCache = new();
	private static readonly ConcurrentDictionary<(AgeGradeCalculator.Category, byte, TrackEvent, TimeSpan), double> TrackAgeGradeCache = new();
	private static readonly ConcurrentDictionary<(AgeGradeCalculator.Category, byte, FieldEvent, double), double> FieldAgeGradeCache = new();

	[JsonIgnore]
	public double? AgeGrade
	{
		get
		{
			if (Duration is null && Distance is null)
				return null;

			var category = Athlete.Category?.Value ?? Category.M.Value;
			var age = AgeOnDayOfRun;

			return Course.Race.Type == "Track"
				? TrackAgeGrade(category, age)
				: RoadAgeGrade(category, age);
		}
	}

	private double? TrackAgeGrade(AgeGradeCalculator.Category category, byte age)
		=> Enum.TryParse<TrackEvent>($"_{CourseName}", out var trackEvent) ? TrackAgeGrade(category, age, trackEvent)
			: Enum.TryParse<FieldEvent>(CourseName.Replace(" ", ""), out var fieldEvent) ? FieldAgeGrade(category, age, fieldEvent)
			: null;

	private double? RoadAgeGrade(AgeGradeCalculator.Category category, byte age)
	{
		var distance = Course.Distance.Meters;
		var duration = Duration.Value;
		var key = (category, age, distance, duration);

		return RoadAgeGradeCache.TryGetValue(key, out var ageGrade)
			? ageGrade
			: RoadAgeGradeCache[key] = AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(category, age, distance, duration);
	}

	private double? TrackAgeGrade(AgeGradeCalculator.Category category, byte age, TrackEvent trackEvent)
	{
		var duration = Duration.Value;
		var key = (category, age, trackEvent, duration);

		try
		{
			return TrackAgeGradeCache.TryGetValue(key, out var ageGrade)
				? ageGrade
				: TrackAgeGradeCache[key] = AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(category, age, trackEvent, duration);
		}
		catch (Exception)
		{
			return null;
		}
	}

	private double? FieldAgeGrade(AgeGradeCalculator.Category category, byte age, FieldEvent fieldEvent)
	{
		var distance = Distance.Meters;
		var key = (category, age, fieldEvent, distance);

		return FieldAgeGradeCache.TryGetValue(key, out var ageGrade)
			? ageGrade
			: FieldAgeGradeCache[key] = AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(category, age, fieldEvent, distance);
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