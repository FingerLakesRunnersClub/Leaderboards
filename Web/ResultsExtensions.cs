using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Reports;

namespace FLRC.Leaderboards.Web;

public static class ResultsExtensions
{
	extension(ICollection<Model.Result> results)
	{
		public RankedList<Time, Model.Result> Fastest(Filter filter = null)
			=> results.Rank(filter ?? new Filter(), rs => rs.Any(r => r.Duration > TimeSpan.Zero || r.Athlete.IsPrivate), rs => rs.OrderBy(r => r.Duration).First(), rs => new Time(rs.Min(r => r.Duration > TimeSpan.Zero ? r.Duration : TimeSpan.MaxValue)));

		public RankedList<Time, Model.Result> BestAverage(Filter filter = null)
		{
			filter ??= new Filter();
			var threshold = results.AverageThreshold(filter);
			return results.Rank(filter, rs => !rs.Key.IsPrivate && rs.Count() >= threshold, rs => rs.Average(rs.First().Course, threshold), rs => new Time(rs.Average(rs.First().Course, threshold).Duration));
		}

		public ushort AverageThreshold(Filter filter = null)
		{
			filter ??= new Filter();
			var groupedResults = results.GroupedResults(filter);
			return groupedResults.Length != 0
				? (ushort)Math.Ceiling(groupedResults.Average(r => r.Count()))
				: (ushort)0;
		}

		public RankedList<ushort, Model.Result> MostRuns(Filter filter = null)
			=> results.RankDescending(filter ?? new Filter(), _ => true, r => r.Average(r.First().Course), r => (ushort)r.Count());

		public RankedList<Miles, Model.Result> MostMiles(Filter filter = null)
			=> results.RankDescending(filter ?? new Filter(), _ => true, r => r.Average(r.First().Course), r => new Miles(r.Count() * new Distance(r.First().Course.DistanceDisplay).Miles));

		public RankedList<Date, Model.Result> Earliest(Filter filter = null)
			=> results.Rank(filter ?? new Filter(), _ => true, g => g.MinBy(r => r.FinishTime), g => new Date(g.Min(r => r.FinishTime)));

		private RankedList<T, Model.Result> Rank<T>(Filter filter, Func<GroupedModelResult, bool> groupFilter, Func<GroupedModelResult, Model.Result> getResult, Func<GroupedModelResult, T> sort)
			=> RankedList(results.GroupedResults(filter).Where(groupFilter).OrderBy(sort), getResult, sort);

		private RankedList<T, Model.Result> RankDescending<T>(Filter filter, Func<GroupedModelResult, bool> groupFilter, Func<GroupedModelResult, Model.Result> getResult, Func<GroupedModelResult, T> sort)
			=> RankedList(results.GroupedResults(filter).Where(groupFilter).OrderByDescending(sort), getResult, sort);

		private static RankedList<T, Model.Result> RankedList<T>(IOrderedEnumerable<GroupedModelResult> sorted, Func<GroupedModelResult, Model.Result> getResult, Func<GroupedModelResult, T> getValue)
		{
			var ranks = new RankedList<T, Model.Result>();
			byte skippedRanks = 0;

			var list = sorted.ThenBy(rs => getResult(rs).Duration).ToArray();
			for (ushort rank = 1; rank <= list.Length; rank++)
			{
				var group = list[rank - 1];
				var result = getResult(group);

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

		public GroupedModelResult[] GroupedResults(Filter filter = null)
			=> results.Filter(filter)
				.GroupBy(r => r.Athlete).Select(g => new GroupedModelResult(g))
				.ToArray();

		private Model.Result[] Filter(Filter filter)
			=> results.Where(r => filter == null || filter.IsMatch(r)).ToArray();

		public Statistics Statistics()
		{
			var allAthletes = results.GroupedResults();
			var fAthletes = results.GroupedResults(new Filter(Category.F));
			var mAthletes = results.GroupedResults(new Filter(Category.M));

			var allResultCount = results.Count;
			var fResultCount = results.Count(r => r.Athlete.Category == 'F');
			var mResultCount = results.Count(r => r.Athlete.Category == 'M');

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
					{ string.Empty, allResultCount * new Distance(results.FirstOrDefault()?.Course.DistanceDisplay ?? "0").Miles },
					{ Category.F.Display, fResultCount * new Distance(results.FirstOrDefault()?.Course.DistanceDisplay ?? "0").Miles },
					{ Category.M.Display, mResultCount * new Distance(results.FirstOrDefault()?.Course.DistanceDisplay ?? "0").Miles }
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
}