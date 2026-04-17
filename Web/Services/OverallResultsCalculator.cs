using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Model;
using Course = FLRC.Leaderboards.Model.Course;

namespace FLRC.Leaderboards.Web.Services;

public sealed class OverallResultsCalculator
{
	private readonly IEnumerable<Course> _courses;
	private readonly IEnumerable<Course> _officialCourses;
	private readonly DateTime _start;
	private readonly Iteration _iteration;

	public OverallResultsCalculator(Iteration iteration)
	{
		_iteration = iteration;
		_courses = _iteration.Races.SelectMany(r => r.Courses);
		_officialCourses = _iteration.OfficialChallenge?.Courses ?? [];
		_start = _iteration.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
	}

	public RankedList<Points, Result> MostPoints(Filter filter = null)
		=> RankedList(_officialCourses.SelectMany(c => c.Results.For(_iteration).Fastest(filter)).GroupBy(r => r.Result.Athlete).Where(g => !g.Key.IsPrivate), g => !g.Key.IsPrivate ? new Points(g.Sum(r => r.Points?.Value ?? 0)) : null, g => g.Sum(r => r.Points?.Value));

	public RankedList<Points, Result> MostPoints(byte limit, Filter filter = null)
		=> RankedList(_officialCourses.SelectMany(c => c.Results.For(_iteration).Fastest(filter)).GroupBy(r => r.Result.Athlete).Where(g => !g.Key.IsPrivate), g => !g.Key.IsPrivate ? new Points(g.OrderByDescending(r => r.Points).Take(limit).Sum(r => r.Points?.Value ?? 0)) : null, g => g.OrderByDescending(r => r.Points).Take(limit).Sum(r => r.Points?.Value ?? 0), g => g.Where(r => r.Rank.Value == 1).Sum(r => r.All.Count > 1 ? r.All[1].BehindLeader.Value.TotalSeconds : 0), g => uint.Min((uint)g.Count(), limit));

	public RankedList<AgeGrade, Result> AgeGrade(Filter filter = null)
		=> RankedList(_officialCourses.SelectMany(c => c.Results.For(_iteration).Fastest(filter)).GroupBy(r => r.Result.Athlete).Where(g => !g.Key.IsPrivate), g => !g.Key.IsPrivate ? new AgeGrade(g.AvgAgeGrade()) : null, g => g.Count(), g => (uint)g.Count());

	public RankedList<Date, Result> Completed(Filter filter = null)
		=> RankedList(_officialCourses.SelectMany(c => c.Results.For(_iteration).Earliest(filter)).GroupBy(r => r.Result.Athlete).Where(a => a.Count() == _officialCourses.Count()), g => g.Max(r => r.Value), g => _start.Subtract(g.Max(r => r.Value)?.Value ?? _start), g => (uint)g.Count());

	public RankedList<Miles, Result> MostMiles(Filter filter = null)
		=> RankedList(_courses.SelectMany(c => c.Results.For(_iteration).MostMiles(filter)).GroupBy(r => r.Result.Athlete), g => new Miles(g.Sum(r => r.Value.Value)), g => new Points(g.Sum(r => r.Value.Value)), g => (uint)g.Sum(r => r.Count));

	public RankedList<int, Result> MostCourses(Filter filter = null)
		=> RankedList(_courses.SelectMany(c => c.Results.For(_iteration).Fastest(filter)).GroupBy(r => r.Result.Athlete), g => g.Count(), g => g.Count(), g => (uint)g.Count());

	private RankedList<T1, Result> RankedList<T1, T2, T3>(IEnumerable<IGrouping<Athlete, Ranked<T2, Result>>> results, Func<IGrouping<Athlete, Ranked<T2, Result>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2, Result>>, T3> sort)
		=> RankedList(results, getValue, sort, getValue, g => (uint)g.Count());

	private RankedList<T1, Result> RankedList<T1, T2, T3>(IEnumerable<IGrouping<Athlete, Ranked<T2, Result>>> results, Func<IGrouping<Athlete, Ranked<T2, Result>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2, Result>>, T3> sort, Func<IGrouping<Athlete, Ranked<T2, Result>>, uint> count)
		=> RankedList(results, getValue, sort, getValue, count);

	private RankedList<T1, Result> RankedList<T1, T2, T3, T4>(IEnumerable<IGrouping<Athlete, Ranked<T2, Result>>> results, Func<IGrouping<Athlete, Ranked<T2, Result>>, T1> getValue, Func<IGrouping<Athlete, Ranked<T2, Result>>, T3> sort, Func<IGrouping<Athlete, Ranked<T2, Result>>, T4> tiebreaker, Func<IGrouping<Athlete, Ranked<T2, Result>>, uint> count)
	{
		var ranks = new RankedList<T1, Result>();
		var list = results.OrderByDescending(sort).ThenByDescending(tiebreaker).ToArray();
		for (ushort rank = 1; rank <= list.Length; rank++)
		{
			var result = list[rank - 1];
			var value = getValue(result);
			var notInFirstPlace = ranks.Any();
			var lastPlace = notInFirstPlace
				? ranks[^1]
				: null;

			var avgAgeGrade = result.AvgAgeGrade();
			var overallResult = new Result
			{
				Athlete = result.Key,
				StartTime = _iteration.StartDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue
			};
			ranks.Add(new Ranked<T1, Result>
			{
				All = ranks,
				Rank = notInFirstPlace && lastPlace.Value.Equals(value) ? lastPlace.Rank : new Rank(rank),
				Result = overallResult,
				Count = count(result),
				AgeGrade = !result.Key.IsPrivate
					? new AgeGrade(avgAgeGrade)
					: null,
				Value = value
			});
		}

		return ranks;
	}
}