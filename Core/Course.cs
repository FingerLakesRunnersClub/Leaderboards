using System.Collections.Concurrent;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core;

public class Course
{
	public const double MetersPerMile = 1609.344;

	public uint ID { get; init; }
	public string Name { get; init; }
	public string Type { get; init; }
	public string Source { get; init; }
	public string Distance { get; init; }
	public double Meters { get; set; }
	public string URL { get; init; }

	public DateTime LastUpdated { get; set; }
	public int LastHash { get; set; }

	public double Miles => Meters / MetersPerMile;

	private IEnumerable<Result> _results = Array.Empty<Result>();

	public IEnumerable<Result> Results
	{
		get => _results;
		set
		{
			_results = value;
			resetCaches();
		}
	}

	public IEnumerable<Result> ResultsAsOf(DateTime date)
		=> Results.Where(r => r.StartTime.Value <= date);

	private void resetCaches()
	{
		_fastestCache.Clear();
		_averageCache.Clear();
		_mostRunsCache.Clear();
		_mostMilesCache.Clear();
		_earliestCache.Clear();
		_thresholdCache.Clear();
		_teamCache = null;
	}

	private readonly IDictionary<string, RankedList<Time>> _fastestCache = new ConcurrentDictionary<string, RankedList<Time>>();

	public RankedList<Time> Fastest(Category category = null, byte? ag = null)
	{
		var key = category?.Display ?? "X";
		if (ag == null && _fastestCache.ContainsKey(key))
			return _fastestCache[key];

		var results = Rank(category, rs => ag == null || rs.Key.Team.Value == ag,
			rs => rs.OrderBy(r => r.Duration).First(), rs => rs.Min(r => r.Duration));

		if (ag != null)
			return results;

		return _fastestCache[key] = results;
	}

	private readonly IDictionary<string, RankedList<Time>> _averageCache = new ConcurrentDictionary<string, RankedList<Time>>();

	public RankedList<Time> BestAverage(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_averageCache.ContainsKey(key))
			return _averageCache[key];

		return _averageCache[key] = Rank(category, rs => rs.Count() >= AverageThreshold(category),
			rs => rs.Average(AverageThreshold(category)), rs => rs.Average(AverageThreshold(category)).Duration);
	}

	private readonly IDictionary<string, RankedList<ushort>> _mostRunsCache = new ConcurrentDictionary<string, RankedList<ushort>>();

	public RankedList<ushort> MostRuns(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_mostRunsCache.ContainsKey(key))
			return _mostRunsCache[key];

		return _mostRunsCache[key] = RankDescending(category, _ => true, r => r.Average(), r => (ushort)r.Count());
	}

	private readonly IDictionary<string, RankedList<double>> _mostMilesCache = new ConcurrentDictionary<string, RankedList<double>>();

	public RankedList<double> MostMiles(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_mostMilesCache.ContainsKey(key))
			return _mostMilesCache[key];

		return _mostMilesCache[key] = RankDescending(category, _ => true, r => r.Average(), r => r.Count() * Miles);
	}

	private readonly IDictionary<string, RankedList<Date>> _earliestCache = new ConcurrentDictionary<string, RankedList<Date>>();

	public RankedList<Date> Earliest(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_earliestCache.ContainsKey(key))
			return _earliestCache[key];

		return _earliestCache[key] = Rank(category, _ => true, g => g.OrderBy(r => r.StartTime).FirstOrDefault(), g => g.Min(r => r.StartTime));
	}

	public IEnumerable<GroupedResult> GroupedResults(Category category = null)
		=> Results.Where(r => category == null || (r.Athlete.Category == category))
			.GroupBy(r => r.Athlete).Select(g => new GroupedResult(g));

	private readonly IDictionary<string, ushort> _thresholdCache = new ConcurrentDictionary<string, ushort>();

	public ushort AverageThreshold(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_thresholdCache.ContainsKey(key))
			return _thresholdCache[key];

		var groupedResults = GroupedResults(category).ToList();
		return _thresholdCache[key] = groupedResults.Any()
			? (ushort)Math.Ceiling(groupedResults.Average(r => r.Count()))
			: (ushort)0;
	}

	private IEnumerable<TeamResults> _teamCache;

	public IEnumerable<TeamResults> TeamPoints()
	{
		if (_teamCache != null)
			return _teamCache;

		var teamResults = GroupedResults()
			.GroupBy(g => g.Key.Team)
			.Select(t => new TeamResults
			{
				Team = t.Key,
				AverageAgeGrade = new AgeGrade(t.OrderBy(rs => rs.Min(r => r.Duration))
					.Take(10)
					.Select(rs =>
						AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(
							rs.First().Athlete.Category?.Value ?? Category.M.Value,
							rs.First().AgeOnDayOfRun,
							Meters,
							rs.Min(r => r.Duration.Value)))
					.Average()
				),
				TotalRuns = (ushort)t.Sum(rs => rs.Count())
			}).ToList();

		var fastestTeams = teamResults.OrderByDescending(t => t.AverageAgeGrade).ToArray();
		for (var x = 0; x < fastestTeams.Length; x++)
			fastestTeams[x].AgeGradePoints =
				x > 0 && fastestTeams[x].AverageAgeGrade == fastestTeams[x - 1].AverageAgeGrade
					? fastestTeams[x - 1].AgeGradePoints
					: (byte)(x + 1);

		var mostRunTeams = teamResults.OrderByDescending(t => t.TotalRuns).ToArray();
		for (var x = 0; x < mostRunTeams.Length; x++)
			mostRunTeams[x].MostRunsPoints =
				x > 0 && mostRunTeams[x].TotalRuns == mostRunTeams[x - 1].TotalRuns
					? mostRunTeams[x - 1].MostRunsPoints
					: (byte)(x + 1);

		return _teamCache = teamResults.Rank();
	}

	private RankedList<T> Rank<T>(Category category, Func<GroupedResult, bool> filter,
		Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
		=> RankedList(GroupedResults(category).Where(filter).OrderBy(sort), getResult, sort);

	private RankedList<T> RankDescending<T>(Category category, Func<GroupedResult, bool> filter,
		Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
		=> RankedList(GroupedResults(category).Where(filter).OrderByDescending(sort), getResult, sort);

	private RankedList<T> RankedList<T>(IOrderedEnumerable<GroupedResult> sorted,
		Func<GroupedResult, Result> getResult, Func<GroupedResult, T> getValue)
	{
		var ranks = new RankedList<T>();

		var list = sorted.ThenBy(rs => getResult(rs).Duration).ToList();
		for (ushort rank = 1; rank <= list.Count; rank++)
		{
			var results = list[rank - 1];
			var athlete = results.Key;
			var category = athlete.Category?.Value ?? Category.M.Value;

			var result = getResult(results);
			var ageGrade = AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(category, result.AgeOnDayOfRun, Meters, result.Duration.Value);

			if (ageGrade >= 100)
				continue;

			var firstPlace = !ranks.Any();
			var value = getValue(results);

			ranks.Add(new Ranked<T>
			{
				Rank = !firstPlace && ranks.Last().Value.Equals(value)
					? ranks.Last().Rank
					: new Rank((ushort)(firstPlace ? 1 : rank)),
				Result = result,
				Value = value,
				Count = (uint)results.Count(),
				BehindLeader = firstPlace ? new Time(TimeSpan.Zero) : result.Behind(ranks.First().Result),
				Points = new Points(firstPlace
					? 100
					: ranks.First().Result.Duration.Value.TotalSeconds / result.Duration.Value.TotalSeconds * 100),
				AgeGrade = new AgeGrade(ageGrade)
			});
		}

		return new RankedList<T>(ranks.OrderBy(r => r.Rank).ThenByDescending(r => r.AgeGrade).ToList());
	}

	public Statistics Statistics() => new()
	{
		Participants = new Dictionary<string, int>
			{
				{string.Empty, GroupedResults().Count()},
				{Category.F.Display, GroupedResults(Category.F).Count()},
				{Category.M.Display, GroupedResults(Category.M).Count()}
			},
		Runs = new Dictionary<string, int>
			{
				{string.Empty, Results.Count()},
				{Category.F.Display, Results.Count(r => r.Athlete.Category == Category.F)},
				{Category.M.Display, Results.Count(r => r.Athlete.Category == Category.M)}
			},
		Miles = new Dictionary<string, double>
			{
				{string.Empty, Results.Count() * Miles },
				{Category.F.Display, Results.Count(r => r.Athlete.Category == Category.F) * Miles },
				{Category.M.Display, Results.Count(r => r.Athlete.Category == Category.M) * Miles }
			},
		Average = new Dictionary<string, double>
			{
				{string.Empty, GroupedResults().Any() ? GroupedResults().Average(a => a.Count()) : 0 },
				{Category.F.Display, GroupedResults(Category.F).Any() ? GroupedResults(Category.F).Average(a => a.Count()) : 0 },
				{Category.M.Display, GroupedResults(Category.M).Any() ? GroupedResults(Category.M).Average(a => a.Count()) : 0 }
			}
	};
}