using System.Collections.Concurrent;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Races;

public sealed class Course
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

	private void resetCaches()
	{
		_fastestCache.Clear();
		_averageCache.Clear();
		_mostRunsCache.Clear();
		_mostMilesCache.Clear();
		_earliestCache.Clear();
		_thresholdCache.Clear();
		_teamCache.Clear();
		ClearCommunityCache();
	}

	public void ClearCommunityCache()
		=> _communityCache.Clear();

	private readonly ConcurrentDictionary<Filter, RankedList<Time>> _fastestCache = new();

	public RankedList<Time> Fastest(Filter filter = null)
	{
		filter ??= new Filter();
		return _fastestCache.TryGetValue(filter, out var results)
			? results
			: _fastestCache[filter] = Rank(filter, _ => true, rs => rs.OrderBy(r => r.Duration).First(), rs => rs.Min(r => r.Duration ?? Time.Max));
	}

	private readonly ConcurrentDictionary<Filter, RankedList<Time>> _averageCache = new();

	public RankedList<Time> BestAverage(Filter filter = null)
	{
		filter ??= new Filter();
		var threshold = AverageThreshold(filter);

		return _averageCache.TryGetValue(filter, out var results)
			? results
			: _averageCache[filter] = Rank(filter, rs => !rs.Key.Private && rs.Count() >= threshold, rs => rs.Average(this, threshold), rs => rs.Average(this, threshold).Duration ?? Time.Max);
	}

	private readonly ConcurrentDictionary<Filter, RankedList<ushort>> _mostRunsCache = new();

	public RankedList<ushort> MostRuns(Filter filter = null)
	{
		filter ??= new Filter();
		return _mostRunsCache.TryGetValue(filter, out var results)
			? results
			: _mostRunsCache[filter] = RankDescending(filter, _ => true, r => r.Average(this), r => (ushort) r.Count());
	}

	private readonly ConcurrentDictionary<Filter, RankedList<Miles>> _mostMilesCache = new();

	public RankedList<Miles> MostMiles(Filter filter = null)
	{
		filter ??= new Filter();
		return _mostMilesCache.TryGetValue(filter, out var results)
			? results
			: _mostMilesCache[filter] = RankDescending(filter, _ => true, r => r.Average(this), r => new Miles(r.Count() * Distance.Miles));
	}

	private readonly ConcurrentDictionary<Filter, RankedList<Date>> _earliestCache = new();

	public RankedList<Date> Earliest(Filter filter = null)
	{
		filter ??= new Filter();
		return _earliestCache.TryGetValue(filter, out var results)
			? results
			: _earliestCache[filter] = Rank(filter, _ => true, g => g.MinBy(r => r.FinishTime), g => g.Min(r => r.FinishTime));
	}

	private readonly ConcurrentDictionary<Filter, RankedList<Stars>> _communityCache = new();

	public RankedList<Stars> CommunityStars(Filter filter = null)
	{
		filter ??= new Filter();
		return _communityCache.TryGetValue(filter, out var results)
			? results
			: _communityCache[filter] = RankDescending(filter, g => g.Sum(r => r.CommunityStars?.Count(s => s.Value)) > 0, g => g.Average(this), g => new Stars((ushort) g.Sum(r => r.CommunityStars.Count(p => p.Value))));
	}

	public IReadOnlyCollection<GroupedResult> GroupedResults(Filter filter = null)
		=> _results.Filter(filter)
			.GroupBy(r => r.Athlete).Select(g => new GroupedResult(g))
			.ToArray();

	private readonly ConcurrentDictionary<Filter, ushort> _thresholdCache = new();

	public ushort AverageThreshold(Filter filter = null)
	{
		filter ??= new Filter();
		if (_thresholdCache.TryGetValue(filter, out var threshold))
		{
			return threshold;
		}

		var groupedResults = GroupedResults(filter);
		return _thresholdCache[filter] = groupedResults.Count != 0
				? (ushort) Math.Ceiling(groupedResults.Average(r => r.Count()))
				: (ushort) 0;
	}

	private readonly ConcurrentDictionary<Filter, RankedList<TeamResults>> _teamCache = new();

	public RankedList<TeamResults> TeamPoints(Filter filter = null)
	{
		filter ??= new Filter();
		return _teamCache.TryGetValue(filter, out var results)
			? results
			: _teamCache[filter] = RankedTeamResults(filter);
	}

	private RankedList<TeamResults> RankedTeamResults(Filter filter)
	{
		var teamResults = GroupedResults(filter)
			.GroupBy(g => g.Key.Team)
			.Select(t => new TeamResults
			{
				Team = t.Key,
				AverageAgeGrade = new AgeGrade(t.Select(rs => rs.MinBy(r => r.Duration))
						.Where(r => r?.Duration != null)
						.OrderBy(r => r.Duration)
						.Take(10)
						.Sum(r => r.AgeGrade ?? 0) / 10
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

	private static RankedList<T> RankedList<T>(IOrderedEnumerable<GroupedResult> sorted, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> getValue)
	{
		var ranks = new RankedList<T>();

		var list = sorted.ThenBy(rs => getResult(rs).Duration ?? Time.Max).ToArray();
		for (ushort rank = 1; rank <= list.Length; rank++)
		{
			var results = list[rank - 1];
			var result = getResult(results);

			if (result.AgeGrade >= 100)
				continue;

			var isInFirstPlace = !ranks.Any(r => r.Result.Duration is not null);
			var value = getValue(results);

			var firstPlace = !isInFirstPlace ? ranks.First(r => r.Result.Duration is not null) : null;
			var lastPlace = !isInFirstPlace ? ranks.Last() : null;

			var rankedResult = new Ranked<T>
			{
				All = ranks,
				Rank = !isInFirstPlace && lastPlace.Value.Equals(value)
					? lastPlace.Rank
					: new Rank((ushort) (isInFirstPlace ? 1 : rank)),
				Result = result,
				Value = value,
				Count = (uint) results.Count(),
				BehindLeader = isInFirstPlace ? new Time(TimeSpan.Zero) : result.Behind(firstPlace.Result),
				Points = result.Duration is not null
					? new Points(isInFirstPlace ? 100 : firstPlace.Result.Duration.Value.TotalSeconds / result.Duration.Value.TotalSeconds * 100)
					: null,
				AgeGrade = result.AgeGrade is not null
					? new AgeGrade(result.AgeGrade.Value)
					: null
			};

			ranks.Add(rankedResult);
		}

		return new RankedList<T>(ranks.OrderBy(r => r.Rank).ThenByDescending(r => r.AgeGrade).ToArray());
	}

	public Statistics Statistics() => new()
	{
		Participants = new Dictionary<string, int>
		{
			{ string.Empty, GroupedResults().Count },
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
			{ string.Empty, GroupedResults().Count != 0 ? GroupedResults().Average(a => a.Count()) : 0 },
			{ Category.F.Display, GroupedResults(Filter.F).Count != 0 ? GroupedResults(Filter.F).Average(a => a.Count()) : 0 },
			{ Category.M.Display, GroupedResults(Filter.M).Count != 0 ? GroupedResults(Filter.M).Average(a => a.Count()) : 0 }
		}
	};
}