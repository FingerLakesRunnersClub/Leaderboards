using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;

namespace FLRC.Leaderboards.Core.Races;

public static class CourseExtensions
{
	extension(Course course)
	{
		public RankedList<Time, Result> Fastest(Filter filter = null)
			=> course.Rank(filter ?? new Filter(), rs => rs.Any(r => r.Duration is not null || r.Athlete.Private), rs => rs.OrderBy(r => r.Duration).First(), rs => rs.Min(r => r.Duration ?? Time.Max));

		private RankedList<T, Result> Rank<T>(Filter filter, Func<GroupedResult, bool> groupFilter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
			=> RankedList(course.GroupedResults(filter).Where(groupFilter).OrderBy(sort), getResult, sort);

		private RankedList<T, Result> RankDescending<T>(Filter filter, Func<GroupedResult, bool> groupFilter, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> sort)
			=> RankedList(course.GroupedResults(filter).Where(groupFilter).OrderByDescending(sort), getResult, sort);

		public RankedList<Stars, Result> CommunityStars(Filter filter = null)
			=> course.RankDescending(filter ?? new Filter(), g => g.Sum(r => r.CommunityStars?.Count(s => s.Value)) > 0, g => g.Average(course), g => new Stars((ushort)g.Sum(r => r.CommunityStars.Count(p => p.Value))));

		public GroupedResult[] GroupedResults(Filter filter = null)
			=> course.Results.Filter(filter)
				.GroupBy(r => r.Athlete).Select(g => new GroupedResult(g))
				.ToArray();

		public RankedList<TeamResults, Result> TeamPoints(Filter filter = null)
			=> course.RankedTeamResults(filter ?? new Filter());

		private RankedList<TeamResults, Result> RankedTeamResults(Filter filter)
		{
			var teamResults = course.GroupedResults(filter)
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
					TotalRuns = (ushort)t.Sum(rs => rs.Count())
				}).ToArray();

			var fastestTeams = teamResults.OrderByDescending(t => t.AverageAgeGrade).ToArray();
			for (var x = 0; x < fastestTeams.Length; x++)
				fastestTeams[x].AgeGradePoints = x > 0 && fastestTeams[x].AverageAgeGrade == fastestTeams[x - 1].AverageAgeGrade
					? fastestTeams[x - 1].AgeGradePoints
					: (byte)(x + 1);

			var mostRunTeams = teamResults.OrderByDescending(t => t.TotalRuns).ToArray();
			for (var x = 0; x < mostRunTeams.Length; x++)
				mostRunTeams[x].MostRunsPoints = x > 0 && mostRunTeams[x].TotalRuns == mostRunTeams[x - 1].TotalRuns
					? mostRunTeams[x - 1].MostRunsPoints
					: (byte)(x + 1);

			return teamResults.Rank();
		}

		public Time FormatTime(TimeSpan time)
			=> course.ShowDecimals
				? new SprintTime(time)
				: new Time(time);

		public ushort AverageThreshold(Filter filter = null)
		{
			filter ??= new Filter();
			var groupedResults = course.GroupedResults(filter);
			return groupedResults.Length != 0
				? (ushort)Math.Ceiling(groupedResults.Average(r => r.Count()))
				: (ushort)0;
		}

		public RankedList<Performance, Result> Farthest(Filter filter = null)
			=> course.RankDescending(filter ?? new Filter(), rs => rs.Any(r => r.Performance is not null), rs => rs.OrderByDescending(r => r.Performance).First(), rs => rs.Max(r => r.Performance ?? Performance.Zero));

		public RankedList<Time, Result> BestAverage(Filter filter = null)
		{
			filter ??= new Filter();
			var threshold = course.AverageThreshold(filter);
			return course.Rank(filter, rs => !rs.Key.Private && rs.Count() >= threshold, rs => rs.Average(course, threshold), rs => rs.Average(course, threshold).Duration ?? Time.Max);
		}

		public RankedList<ushort, Result> MostRuns(Filter filter = null)
			=> course.RankDescending(filter ?? new Filter(), _ => true, r => r.Average(course), r => (ushort)r.Count());

		public RankedList<Miles, Result> MostMiles(Filter filter = null)
			=> course.RankDescending(filter ?? new Filter(), _ => true, r => r.Average(course), r => new Miles(r.Count() * course.Distance.Miles));

		public RankedList<Date, Result> Earliest(Filter filter = null)
			=> course.Rank(filter ?? new Filter(), _ => true, g => g.MinBy(r => r.FinishTime), g => g.Min(r => r.FinishTime));

		public Statistics Statistics()
		{
			var allAthletes = course.GroupedResults();
			var fAthletes = course.GroupedResults(Filter.F);
			var mAthletes = course.GroupedResults(Filter.M);

			var allResultCount = course.Results.Length;
			var fResultCount = course.Results.Count(r => r.Athlete.Category == Category.F);
			var mResultCount = course.Results.Count(r => r.Athlete.Category == Category.M);

			var averageTotal = allAthletes.Length > 0 ? allAthletes.Average(a => a.Count()) : 0;
			var fAverage = fAthletes.Length > 0 ? fAthletes.Average(a => a.Count()) : 0;
			var mAverage = mAthletes.Length > 0 ? mAthletes.Average(a => a.Count()) : 0;

			return new Statistics
			{
				Participants = new Dictionary<string, int>
				{
					{ string.Empty, allAthletes.Length },
					{ Category.F.Display, fAthletes.Length },
					{ Category.M.Display, mAthletes.Length }
				},
				Runs = new Dictionary<string, int>
				{
					{ string.Empty, allResultCount },
					{ Category.F.Display, fResultCount },
					{ Category.M.Display, mResultCount }
				},
				Miles = new Dictionary<string, double>
				{
					{ string.Empty, allResultCount * course.Distance?.Miles ?? 0 },
					{ Category.F.Display, fResultCount * course.Distance?.Miles ?? 0 },
					{ Category.M.Display, mResultCount * course.Distance?.Miles ?? 0 }
				},
				Average = new Dictionary<string, double>
				{
					{ string.Empty, allAthletes.Length != 0 ? averageTotal : 0 },
					{ Category.F.Display, fAthletes.Length != 0 ? fAverage : 0 },
					{ Category.M.Display, mAthletes.Length != 0 ? mAverage : 0 }
				}
			};
		}
	}

	extension(Result result)
	{
		public Time BehindLeader<T>(bool isInFirstPlace, Ranked<T, Result> firstPlace)
			=> isInFirstPlace || result.Duration is null || firstPlace?.Result.Duration is null
				? result.Course.FormatTime(TimeSpan.Zero)
				: result.Behind(firstPlace.Result);

		public Points Points<T>(bool isInFirstPlace, Ranked<T, Result> firstPlace)
			=> result.Duration is not null && (isInFirstPlace || firstPlace?.Result.Duration is not null)
				? new Points(isInFirstPlace ? 100 : firstPlace.Result.Duration.Value.TotalSeconds / result.Duration.Value.TotalSeconds * 100)
				: null;

		public AgeGrade AgeGrade()
			=> result.AgeGrade is not null
				? new AgeGrade(result.AgeGrade.Value)
				: null;
	}

	private static RankedList<T, Result> RankedList<T>(IOrderedEnumerable<GroupedResult> sorted, Func<GroupedResult, Result> getResult, Func<GroupedResult, T> getValue)
	{
		var ranks = new RankedList<T, Result>();
		byte skippedRanks = 0;

		var list = sorted.ThenBy(rs => getResult(rs).Duration ?? Time.Max).ToArray();
		for (ushort rank = 1; rank <= list.Length; rank++)
		{
			var results = list[rank - 1];
			var result = getResult(results);

			var isInFirstPlace = !ranks.Exists(r => r.Value is not null);
			var value = getValue(results);

			var firstPlace = !isInFirstPlace ? ranks.First(r => r.Value is not null) : null;
			var lastPlace = !isInFirstPlace ? ranks[^1] : null;

			var rankedResult = new Ranked<T>
			{
				All = ranks,
				Rank = Rank(isInFirstPlace, lastPlace, value, (ushort)(rank - skippedRanks)),
				Result = result,
				Value = value,
				Count = (uint)results.Count(),
				BehindLeader = result.BehindLeader(isInFirstPlace, firstPlace),
				Points = result.Points(isInFirstPlace, firstPlace),
				AgeGrade = result.AgeGrade()
			};

			ranks.Add(rankedResult);
		}

		return new RankedList<T, Result>(ranks.OrderBy(r => r.Rank).ThenByDescending(r => r.AgeGrade).ToArray());
	}

	private static Rank Rank<T>(bool isInFirstPlace, Ranked<T, Result> lastPlace, T value, ushort rank)
		=> !isInFirstPlace && lastPlace.Value.Equals(value)
			? lastPlace.Rank
			: new Rank((ushort)(isInFirstPlace ? 1 : rank));
}