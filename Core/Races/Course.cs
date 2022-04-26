using System.Collections.Concurrent;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Races;

public class Course
{
	public Race Race { get; init; }
	public uint ID { get; init; }
	public string Name => Race.Name + (Race.Courses?.Count > 1 ? $" ({Distance.Display})" : string.Empty);
	public Distance Distance { get; init; }

	public DateTime LastUpdated { get; set; }
	public int LastHash { get; set; }

	private IReadOnlyCollection<Result> _results = Array.Empty<Result>();

	public IReadOnlyCollection<Result> Results
	{
		get => _results;
		set
		{
			_results = value;
			resetCaches();
		}
	}

	public IReadOnlyCollection<Result> ResultsAsOf(DateTime date)
		=> _results.Where(r => r.StartTime.Value <= date).ToArray();

	private void resetCaches()
	{
		_fastestCache.Clear();
		_averageCache.Clear();
		_mostRunsCache.Clear();
		_mostMilesCache.Clear();
		_earliestCache.Clear();
		_thresholdCache.Clear();
		_teamCache.Clear();
	}

	private readonly IDictionary<Filter, RankedList<Time>> _fastestCache = new ConcurrentDictionary<Filter, RankedList<Time>>();

	public RankedList<Time> Fastest(Filter filter = null)
	{
		filter ??= new Filter();
		return _fastestCache.ContainsKey(filter)
			? _fastestCache[filter]
			: _fastestCache[filter] = Rank(filter, _ => true, rs => rs.OrderBy(r => r.Duration).First(), rs => rs.Min(r => r.Duration));
	}

	private readonly IDictionary<Filter, RankedList<Time>> _averageCache = new ConcurrentDictionary<Filter, RankedList<Time>>();

	public RankedList<Time> BestAverage(Filter filter = null)
	{
		filter ??= new Filter();
		var threshold = AverageThreshold(filter);

		return _averageCache.ContainsKey(filter)
			? _averageCache[filter]
			: _averageCache[filter] = Rank(filter, rs => rs.Count() >= threshold, rs => rs.Average(this, threshold), rs => rs.Average(this, threshold).Duration);
	}

	private readonly IDictionary<Filter, RankedList<ushort>> _mostRunsCache = new ConcurrentDictionary<Filter, RankedList<ushort>>();

	public RankedList<ushort> MostRuns(Filter filter = null)
	{
		filter ??= new Filter();
		return _mostRunsCache.ContainsKey(filter)
			? _mostRunsCache[filter]
			: _mostRunsCache[filter] = RankDescending(filter, _ => true, r => r.Average(this), r => (ushort) r.Count());
	}

	private readonly IDictionary<Filter, RankedList<Miles>> _mostMilesCache = new ConcurrentDictionary<Filter, RankedList<Miles>>();

	public RankedList<Miles> MostMiles(Filter filter = null)
	{
		filter ??= new Filter();
		return _mostMilesCache.ContainsKey(filter)
			? _mostMilesCache[filter]
			: _mostMilesCache[filter] = RankDescending(filter, _ => true, r => r.Average(this), r => new Miles(r.Count() * Distance.Miles));
	}

	private readonly IDictionary<Filter, RankedList<Date>> _earliestCache = new ConcurrentDictionary<Filter, RankedList<Date>>();

	public RankedList<Date> Earliest(Filter filter = null)
	{
		filter ??= new Filter();
		return _earliestCache.ContainsKey(filter)
			? _earliestCache[filter]
			: _earliestCache[filter] = Rank(filter, _ => true, g => g.MinBy(r => r.StartTime), g => g.Min(r => r.StartTime));
	}

	private readonly IDictionary<Filter, RankedList<Stars>> _communityCache = new ConcurrentDictionary<Filter, RankedList<Stars>>();

	public RankedList<Stars> CommunityStars(Filter filter = null)
	{
		filter ??= new Filter();
		return _communityCache.ContainsKey(filter)
			? _communityCache[filter]
			: _communityCache[filter] = RankDescending(filter, _ => true, g => g.Average(this), g => new Stars((ushort) g.Sum(r => r.CommunityStars.Count(p => p.Value))));
	}

	public IReadOnlyCollection<GroupedResult> GroupedResults(Filter filter = null)
		=> _results.Filter(filter)
			.GroupBy(r => r.Athlete).Select(g => new GroupedResult(g))
			.ToArray();

	private readonly IDictionary<Filter, ushort> _thresholdCache = new ConcurrentDictionary<Filter, ushort>();

	public ushort AverageThreshold(Filter filter = null)
	{
		filter ??= new Filter();
		var groupedResults = GroupedResults(filter);

		return _thresholdCache.ContainsKey(filter)
			? _thresholdCache[filter]
			: (_thresholdCache[filter] = groupedResults.Any()
				? (ushort) Math.Ceiling(groupedResults.Average(r => r.Count()))
				: (ushort) 0);
	}

	private readonly IDictionary<Filter, RankedList<TeamResults>> _teamCache = new ConcurrentDictionary<Filter, RankedList<TeamResults>>();

	public RankedList<TeamResults> TeamPoints(Filter filter = null)
	{
		filter ??= new Filter();
		return _teamCache.ContainsKey(filter)
			? _teamCache[filter]
			: _teamCache[filter] = RankedTeamResults(filter);
	}

	private RankedList<TeamResults> RankedTeamResults(Filter filter)
	{
		var teamResults = GroupedResults(filter)
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
							Distance.Meters,
							rs.Min(r => r.Duration.Value)))
					.Average()
				),
				TotalRuns = (ushort) t.Sum(rs => rs.Count())
			}).ToArray();

		var fastestTeams = teamResults.OrderByDescending(t => t.AverageAgeGrade).ToArray();
		for (var x = 0; x < fastestTeams.Length; x++)
			fastestTeams[x].AgeGradePoints = x > 0 && fastestTeams[x].AverageAgeGrade == fastestTeams[x - 1].AverageAgeGrade
				? fastestTeams[x - 1].AgeGradePoints
				: (byte) (x + 1);

		var mostRunTeams = teamResults.OrderByDescending(t => t.TotalRuns).ToArray();
		for (var x = 0; x < mostRunTeams.Length; x++)
			mostRunTeams[x].MostRunsPoints = x > 0 && mostRunTeams[x].TotalRuns == mostRunTeams[x - 1].TotalRuns
				? mostRunTeams[x - 1].MostRunsPoints
				: (byte) (x + 1);

		return teamResults.Rank();
	}

	private RankedList<T> Rank<T>(Filter filter, Func<GroupedResult, bool> groupFilter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
		=> RankedList(GroupedResults(filter).Where(groupFilter).OrderBy(sort), getResult, sort);

	private RankedList<T> RankDescending<T>(Filter filter, Func<GroupedResult, bool> groupFilter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
		=> RankedList(GroupedResults(filter).Where(groupFilter).OrderByDescending(sort), getResult, sort);

	private RankedList<T> RankedList<T>(IOrderedEnumerable<GroupedResult> sorted, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> getValue)
	{
		var ranks = new RankedList<T>();

		var list = sorted.ThenBy(rs => getResult(rs).Duration).ToArray();
		for (ushort rank = 1; rank <= list.Length; rank++)
		{
			var results = list[rank - 1];
			var athlete = results.Key;
			var category = athlete.Category?.Value ?? Category.M.Value;

			var result = getResult(results);
			var ageGrade = AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(category, result.AgeOnDayOfRun, Distance.Meters, result.Duration.Value);

			if (ageGrade >= 100)
				continue;

			var firstPlace = !ranks.Any();
			var value = getValue(results);

			var rankedResult = new Ranked<T>
			{
				All = ranks,
				Rank = !firstPlace && ranks.Last().Value.Equals(value)
					? ranks.Last().Rank
					: new Rank((ushort) (firstPlace ? 1 : rank)),
				Result = result,
				Value = value,
				Count = (uint) results.Count(),
				BehindLeader = firstPlace ? new Time(TimeSpan.Zero) : result.Behind(ranks.First().Result),
				Points = new Points(firstPlace
					? 100
					: ranks.First().Result.Duration.Value.TotalSeconds / result.Duration.Value.TotalSeconds * 100),
				AgeGrade = new AgeGrade(ageGrade)
			};

			ranks.Add(rankedResult);
		}

		return new RankedList<T>(ranks.OrderBy(r => r.Rank).ThenByDescending(r => r.AgeGrade).ToArray());
	}

	public Statistics Statistics() => new()
	{
		Participants = new Dictionary<string, int>
		{
			{ string.Empty, GroupedResults().Count() },
			{ Category.F.Display, GroupedResults(Filter.F).Count },
			{ Category.M.Display, GroupedResults(Filter.M).Count }
		},
		Runs = new Dictionary<string, int>
		{
			{ string.Empty, _results.Count },
			{ Category.F.Display, _results.Count(r => r.Athlete.Category == Category.F) },
			{ Category.M.Display, _results.Count(r => r.Athlete.Category == Category.M) }
		},
		Miles = new Dictionary<string, double>
		{
			{ string.Empty, _results.Count * Distance.Miles },
			{ Category.F.Display, _results.Count(r => r.Athlete.Category == Category.F) * Distance.Miles },
			{ Category.M.Display, _results.Count(r => r.Athlete.Category == Category.M) * Distance.Miles }
		},
		Average = new Dictionary<string, double>
		{
			{ string.Empty, GroupedResults().Any() ? GroupedResults().Average(a => a.Count()) : 0 },
			{ Category.F.Display, GroupedResults(Filter.F).Any() ? GroupedResults(Filter.F).Average(a => a.Count()) : 0 },
			{ Category.M.Display, GroupedResults(Filter.M).Any() ? GroupedResults(Filter.M).Average(a => a.Count()) : 0 }
		}
	};

	public IReadOnlyCollection<DateOnly> DistinctMonths()
		=> _results.Select(r => new DateOnly(r.StartTime.Value.Year, r.StartTime.Value.Month, 1))
			.Distinct()
			.OrderBy(d => d)
			.ToArray();
}