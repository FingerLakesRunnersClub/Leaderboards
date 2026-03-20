using System.Collections.Concurrent;
using FLRC.AgeGradeCalculator;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class ResultsController(ICourseService courseService) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Fastest(Guid id)
	{
		var course = await courseService.Get(id);
		var fastest = course.Results.Fastest();
		var results = new CourseResults<Time>
		{
			Course = course,
			Type = ResultType.Fastest,
			Results = fastest
		};
		var vm = new ViewModel<CourseResults<Time>>($"{course.Race.Name} Results", results);
		return View("List", vm);
	}
}

public static class ResultsExtensions
{
	extension(ICollection<Model.Result> results)
	{
		public RankedList<Time, Model.Result> Fastest(Filter filter = null)
			=> results.Rank(filter ?? new Filter(), rs => rs.Any(r => r.Duration > TimeSpan.Zero || r.Athlete.IsPrivate), rs => rs.OrderBy(r => r.Duration).First(), rs => new Time(rs.Min(r => r.Duration)));

		private RankedList<T, Model.Result> Rank<T>(Filter filter, Func<GroupedModelResult, bool> groupFilter, Func<GroupedModelResult, Model.Result> getResult, Func<GroupedModelResult, T> sort)
			=> RankedList(results.GroupedResults(filter).Where(groupFilter).OrderBy(sort), getResult, sort, false);

		private static RankedList<T, Model.Result> RankedList<T>(IOrderedEnumerable<GroupedModelResult> sorted, Func<GroupedModelResult, Model.Result> getResult, Func<GroupedModelResult, T> getValue, bool allowInvalid)
		{
			var ranks = new RankedList<T, Model.Result>();
			byte skippedRanks = 0;

			var list = sorted.ThenBy(rs => getResult(rs).Duration).ToArray();
			for (ushort rank = 1; rank <= list.Length; rank++)
			{
				var group = list[rank - 1];
				var result = getResult(group);

				if (result.AgeGrade()?.Value > 100 && !allowInvalid)
				{
					skippedRanks++;
					continue;
				}

				var isInFirstPlace = !ranks.Exists(r => r.Value is not null);
				var value = getValue(group);

				var firstPlace = !isInFirstPlace ? ranks.First(r => r.Value is not null) : null;
				var lastPlace = !isInFirstPlace ? ranks[^1] : null;

				var rankedResult = new Ranked<T, Model.Result>
				{
					All = ranks,
					Rank = Rank(isInFirstPlace, lastPlace, value, (ushort)(rank - skippedRanks)),
					Result = result,
					Value = value,
					Count = (uint)group.Count(),
					BehindLeader = result.BehindLeader(isInFirstPlace, firstPlace),
					Points = result.Points(isInFirstPlace, firstPlace),
					AgeGrade = result.AgeGrade()
				};

				ranks.Add(rankedResult);
			}

			return new RankedList<T, Model.Result>(ranks.OrderBy(r => r.Rank).ThenByDescending(r => r.AgeGrade).ToArray());
		}

		private static Rank Rank<T>(bool isInFirstPlace, Ranked<T, Model.Result> lastPlace, T value, ushort rank)
			=> !isInFirstPlace && lastPlace.Value.Equals(value)
				? lastPlace.Rank
				: new Rank((ushort)(isInFirstPlace ? 1 : rank));

		private GroupedModelResult[] GroupedResults(Filter filter = null)
			=> results.Filter(filter)
				.GroupBy(r => r.Athlete).Select(g => new GroupedModelResult(g))
				.ToArray();

		private Model.Result[] Filter(Filter filter)
			=> results.Where(r => filter == null || filter.IsMatch(r)).ToArray();
	}

	private static readonly ConcurrentDictionary<(Category, byte, double, TimeSpan), double> RoadAgeGradeCache = new();

	extension(Model.Result result)
	{
		public Time BehindLeader<T>(bool isInFirstPlace, Ranked<T, Model.Result> firstPlace)
			=> isInFirstPlace || result.Duration == TimeSpan.Zero || firstPlace?.Result.Duration is null
				? new Time(TimeSpan.Zero)
				: result.Behind(firstPlace.Result);

		public Time Behind(Model.Result other)
			=> new(result.Duration.Subtract(other.Duration));

		public Points Points<T>(bool isInFirstPlace, Ranked<T, Model.Result> firstPlace)
			=> result.Duration > TimeSpan.Zero && (isInFirstPlace || firstPlace?.Result.Duration is not null)
				? new Points(isInFirstPlace ? 100 : firstPlace.Result.Duration.TotalSeconds / result.Duration.TotalSeconds * 100)
				: null;

		public AgeGrade AgeGrade()
		{
			if (result.Duration == TimeSpan.Zero || result.AthleteAge is null or 0)
				return null;

			var category = result.Athlete.Category == 'F' ? Category.F : Category.M;

			var ageGrade = result.Course.Race.Type is not ("Track" or "Field")
				? result.RoadAgeGrade(category, result.AthleteAge.Value)
				: null;
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
	}

	extension(Filter filter)
	{
		public bool IsMatch(Model.Result result)
			=> (filter.Category == null || result.Athlete.Category == filter.Category.ToString()[0])
			   && (filter.AgeGroup == null || (result.AthleteAge >= filter.AgeGroup.MinAge && (filter.AgeGroup.MaxAge == null || result.AthleteAge <= filter.AgeGroup.MaxAge)));
	}
}

public sealed class GroupedModelResult : Grouped<Model.Athlete, Model.Result>
{
	public GroupedModelResult(IGrouping<Model.Athlete, Model.Result> group) : base(group)
	{
	}

	public Model.Result Average(Model.Course course, ushort? threshold = null)
	{
		var timedResults = _group.Where(r => r.Duration > TimeSpan.Zero).ToArray();
		var average = timedResults.Length > 0
			? timedResults.OrderBy(r => r.Duration)
				.Take(threshold ?? timedResults.Length)
				.Average(r => r.Duration.TotalSeconds)
			: 0;

		return new Model.Result
		{
			Athlete = Key,
			Course = course,
			Duration = !Key.IsPrivate && timedResults.Length > 0
				? TimeSpan.FromSeconds(average)
				: TimeSpan.Zero
		};
	}

	public override int CompareTo(Grouped<Model.Athlete, Model.Result> other) => _group.Key.ID.CompareTo(other.Key.ID);
}