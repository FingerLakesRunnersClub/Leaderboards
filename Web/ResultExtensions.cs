using System.Collections.Concurrent;
using FLRC.AgeGradeCalculator;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Web;

public static class ResultExtensions
{
	private static readonly ConcurrentDictionary<(Category, byte, double, TimeSpan), double> RoadAgeGradeCache = new();
	private static readonly ConcurrentDictionary<(Category, byte, TrackEvent, TimeSpan), double> TrackAgeGradeCache = new();
	private static readonly ConcurrentDictionary<(Category, byte, FieldEvent, double), double> FieldAgeGradeCache = new();

	extension(Result result)
	{
		public bool IsValid()
			=> result.AgeGrade()?.Value <= 100 && result.StartTime <= DateTime.Now;

		public Time BehindLeader<T>(bool isInFirstPlace, Ranked<T, Result> firstPlace)
			=> isInFirstPlace || result.Duration == TimeSpan.Zero || firstPlace?.Result.Duration is null
				? new Time(TimeSpan.Zero)
				: result.Behind(firstPlace.Result);

		public Time Behind(Result other)
			=> new(result.Duration.Subtract(other.Duration));

		public Points Points<T>(bool isInFirstPlace, Ranked<T, Result> firstPlace)
			=> result.Duration > TimeSpan.Zero && (isInFirstPlace || firstPlace?.Result.Duration is not null)
				? new Points(isInFirstPlace ? 100 : firstPlace.Result.Duration.TotalSeconds / result.Duration.TotalSeconds * 100)
				: null;

		public AgeGrade AgeGrade()
		{
			if (result.Duration == TimeSpan.Zero || result.AthleteAge is null or 0)
				return null;

			var category = result.Athlete.Category == 'F' ? Category.F : Category.M;

			var ageGrade = result.Course.Race.Type is "Track" or "Field"
				? result.TFAgeGrade(category, result.AthleteAge.Value)
				: result.RoadAgeGrade(category, result.AthleteAge.Value);
			return ageGrade is not null ? new AgeGrade(ageGrade.Value) : null;
		}

		private double? RoadAgeGrade(Category category, byte age)
		{
			if (result.Course.Distance is 0 || result.Duration == TimeSpan.Zero)
				return null;

			var distance = new Distance(result.Course.DistanceDisplay);
			var duration = result.Duration;
			var key = (category, age, distance.Meters, duration);

			return RoadAgeGradeCache.TryGetValue(key, out var ageGrade)
				? ageGrade
				: RoadAgeGradeCache[key] = AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(category, age, distance.Meters, duration);
		}

		private double? TFAgeGrade(Category category, byte age)
			=> Enum.TryParse<TrackEvent>(result.Course.Race.Name.ToTrackEvent(), out var trackEvent) ? result.TrackAgeGrade(category, age, trackEvent)
				: Enum.TryParse<FieldEvent>(result.Course.Race.Name.ToFieldEvent(), out var fieldEvent) ? throw new NotImplementedException()
				: result.RoadAgeGrade(category, age);

		private double? TrackAgeGrade(Category category, byte age, TrackEvent trackEvent)
		{
			var duration = result.Duration;
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
	}
}