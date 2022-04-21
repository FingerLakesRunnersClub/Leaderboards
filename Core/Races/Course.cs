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
		=> Results.Where(r => r.StartTime.Value <= date).ToArray();

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

		var results = Rank(category, ag, _ => true, rs => rs.OrderBy(r => r.Duration).First(), rs => rs.Min(r => r.Duration));

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

		var threshold = AverageThreshold(category);
		return _averageCache[key] = Rank(category, null, rs => rs.Count() >= threshold, rs => rs.Average(this, threshold), rs => rs.Average(this, threshold).Duration);
	}

	private readonly IDictionary<string, RankedList<ushort>> _mostRunsCache = new ConcurrentDictionary<string, RankedList<ushort>>();

	public RankedList<ushort> MostRuns(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_mostRunsCache.ContainsKey(key))
			return _mostRunsCache[key];

		return _mostRunsCache[key] = RankDescending(category, null, _ => true, r => r.Average(this), r => (ushort) r.Count());
	}

	private readonly IDictionary<string, RankedList<Miles>> _mostMilesCache = new ConcurrentDictionary<string, RankedList<Miles>>();

	public RankedList<Miles> MostMiles(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_mostMilesCache.ContainsKey(key))
			return _mostMilesCache[key];

		return _mostMilesCache[key] = RankDescending(category, null, _ => true, r => r.Average(this), r => new Miles(r.Count() * Distance.Miles));
	}

	private readonly IDictionary<string, RankedList<Date>> _earliestCache = new ConcurrentDictionary<string, RankedList<Date>>();

	public RankedList<Date> Earliest(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_earliestCache.ContainsKey(key))
			return _earliestCache[key];

		return _earliestCache[key] = Rank(category, null, _ => true, g => g.MinBy(r => r.StartTime), g => g.Min(r => r.StartTime));
	}

	private readonly IDictionary<string, RankedList<Stars>> _communityCache = new ConcurrentDictionary<string, RankedList<Stars>>();

	public RankedList<Stars> CommunityStars(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_earliestCache.ContainsKey(key))
			return _communityCache[key];

		return _communityCache[key] = RankDescending(category, null, _ => true, g => g.Average(this), g => new Stars((ushort) g.Sum(r => r.CommunityStars.Count(p => p.Value))));
	}

	public IReadOnlyCollection<GroupedResult> GroupedResults(Category category = null, byte? ag = null)
		=> Results.Where(r => (category == null || r.Athlete.Category == category) && (ag == null || (r.AgeOnDayOfRun >= Athlete.Teams[ag.Value].MinAge && r.AgeOnDayOfRun <= Athlete.Teams[ag.Value].MaxAge)))
			.GroupBy(r => r.Athlete).Select(g => new GroupedResult(g))
			.ToArray();

	private readonly IDictionary<string, ushort> _thresholdCache = new ConcurrentDictionary<string, ushort>();

	public ushort AverageThreshold(Category category = null)
	{
		var key = category?.Display ?? "X";
		if (_thresholdCache.ContainsKey(key))
			return _thresholdCache[key];

		var groupedResults = GroupedResults(category).ToList();
		return _thresholdCache[key] = groupedResults.Any()
			? (ushort) Math.Ceiling(groupedResults.Average(r => r.Count()))
			: (ushort) 0;
	}

	private IReadOnlyCollection<TeamResults> _teamCache;

	public IReadOnlyCollection<TeamResults> TeamPoints()
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
							Distance.Meters,
							rs.Min(r => r.Duration.Value)))
					.Average()
				),
				TotalRuns = (ushort) t.Sum(rs => rs.Count())
			}).ToList();

		var fastestTeams = teamResults.OrderByDescending(t => t.AverageAgeGrade).ToArray();
		for (var x = 0; x < fastestTeams.Length; x++)
			fastestTeams[x].AgeGradePoints =
				x > 0 && fastestTeams[x].AverageAgeGrade == fastestTeams[x - 1].AverageAgeGrade
					? fastestTeams[x - 1].AgeGradePoints
					: (byte) (x + 1);

		var mostRunTeams = teamResults.OrderByDescending(t => t.TotalRuns).ToArray();
		for (var x = 0; x < mostRunTeams.Length; x++)
			mostRunTeams[x].MostRunsPoints =
				x > 0 && mostRunTeams[x].TotalRuns == mostRunTeams[x - 1].TotalRuns
					? mostRunTeams[x - 1].MostRunsPoints
					: (byte) (x + 1);

		return _teamCache = teamResults.Rank();
	}

	private RankedList<T> Rank<T>(Category category, byte? ag, Func<GroupedResult, bool> filter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
		=> RankedList(GroupedResults(category, ag).Where(filter).OrderBy(sort), getResult, sort);

	private RankedList<T> RankDescending<T>(Category category, byte? ag, Func<GroupedResult, bool> filter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
		=> RankedList(GroupedResults(category, ag).Where(filter).OrderByDescending(sort), getResult, sort);

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

		return new RankedList<T>(ranks.OrderBy(r => r.Rank).ThenByDescending(r => r.AgeGrade).ToList());
	}

	public Statistics Statistics() => new()
	{
		Participants = new Dictionary<string, int>
		{
			{ string.Empty, GroupedResults().Count() },
			{ Category.F.Display, GroupedResults(Category.F).Count() },
			{ Category.M.Display, GroupedResults(Category.M).Count() }
		},
		Runs = new Dictionary<string, int>
		{
			{ string.Empty, Results.Count() },
			{ Category.F.Display, Results.Count(r => r.Athlete.Category == Category.F) },
			{ Category.M.Display, Results.Count(r => r.Athlete.Category == Category.M) }
		},
		Miles = new Dictionary<string, double>
		{
			{ string.Empty, Results.Count() * Distance.Miles },
			{ Category.F.Display, Results.Count(r => r.Athlete.Category == Category.F) * Distance.Miles },
			{ Category.M.Display, Results.Count(r => r.Athlete.Category == Category.M) * Distance.Miles }
		},
		Average = new Dictionary<string, double>
		{
			{ string.Empty, GroupedResults().Any() ? GroupedResults().Average(a => a.Count()) : 0 },
			{ Category.F.Display, GroupedResults(Category.F).Any() ? GroupedResults(Category.F).Average(a => a.Count()) : 0 },
			{ Category.M.Display, GroupedResults(Category.M).Any() ? GroupedResults(Category.M).Average(a => a.Count()) : 0 }
		}
	};
}