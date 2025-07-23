using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Leaders;

public sealed class LeaderboardViewModel : ViewModel
{
	public override string Title => "Leaderboard";

	public LeaderboardResultType LeaderboardResultType { get; }

	private readonly Course[] _courses;
	private readonly byte _tableSize;
	private readonly Func<LeaderboardTable, bool> _leaderboardFilter;

	public LeaderboardViewModel(Course[] courses, LeaderboardResultType type, byte tableSize)
	{
		_courses = courses;
		LeaderboardResultType = type;
		_leaderboardFilter = GetFilter(type);
		_tableSize = tableSize;
	}

	private LeaderboardTable OverallTable(string id, ResultType type, Filter filter, Func<LeaderboardRow[]> rows)
		=> Config.Competitions.TryGetValue(id, out var title)
			? new LeaderboardTable
			{
				Link = $"/Overall/{id}",
				Title = title,
				ResultType = new FormattedResultType(type),
				Filter = filter,
				Rows = new Lazy<LeaderboardRow[]>(rows)
			}
			: null;

	public LeaderboardTable[] OverallResults()
	{
		var vm = new OverallResults(_courses);
		var leaderboards = new List<LeaderboardTable>
		{
			OverallTable("Points/F", ResultType.Fastest, Filter.F, () => vm.MostPoints(Filter.F).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("Points/M", ResultType.Fastest, Filter.M, () => vm.MostPoints(Filter.M).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("PointsTop3/F", ResultType.Fastest, Filter.F, () => vm.MostPoints(_tableSize, Filter.F).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("PointsTop3/M", ResultType.Fastest, Filter.M, () => vm.MostPoints(_tableSize, Filter.M).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("AgeGrade", ResultType.BestAverage, Filter.None, () => vm.AgeGrade(Filter.None).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.AgeGrade.Display })
				.ToArray()),

			OverallTable("Miles", ResultType.MostRuns, Filter.None, () => vm.MostMiles(Filter.None).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("Community", ResultType.Community, Filter.None, () => vm.CommunityStars(Filter.None).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Value.ToString() })
				.ToArray()),

			OverallTable("Team", ResultType.Team, Filter.None, () => vm.TeamPoints(Filter.None).Take(_tableSize)
				.Select(t => new LeaderboardRow { Rank = t.Rank, Name = t.Value.Team.Display, Link = $"/Team/Index/{t.Value.Team.Value}", Value = t.Value.TotalPoints.ToString() })
				.ToArray())
		};

		return leaderboards.Where(l => l != null).ToArray();
	}

	public IDictionary<Course, LeaderboardTable[]> CourseResults
		=> _courses.ToDictionary(c => c, c => LeaderboardTables(c, _tableSize).Where(t => Config.Features.MultiAttemptCompetitions ? _leaderboardFilter(t) : t.ResultType.Value is ResultType.Fastest or ResultType.Farthest)
			.Where(t => t.Rows.Value.Length > 0)
			.ToArray());

	private static LeaderboardTable[] LeaderboardTables(Course course, byte tableSize)
		=> [
			new()
			{
				Title = $"{course.EventSuperlative} (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Filter = Filter.F,
				Link = $"/Course/{course.ID}/{course.ShortName}/{ResultType.Fastest}/F",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Fastest(Filter.F).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = $"{course.EventSuperlative} (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Filter = Filter.M,
				Link = $"/Course/{course.ID}/{course.ShortName}/{ResultType.Fastest}/M",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Fastest(Filter.M).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = $"{course.EventSuperlative} (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Farthest),
				Filter = Filter.F,
				Link = $"/Course/{course.ID}/{course.ShortName}/{ResultType.Farthest}/F",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Farthest(Filter.F).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = $"{course.EventSuperlative} (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Farthest),
				Filter = Filter.M,
				Link = $"/Course/{course.ID}/{course.ShortName}/{ResultType.Farthest}/M",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Farthest(Filter.M).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Best Average (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Filter = Filter.F,
				Link = $"/Course/{course.ID}/{ResultType.BestAverage}/F",
				Rows = new Lazy<LeaderboardRow[]>(() => course.BestAverage(Filter.F).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Best Average (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Filter = Filter.M,
				Link = $"/Course/{course.ID}/{ResultType.BestAverage}/M",
				Rows = new Lazy<LeaderboardRow[]>(() => course.BestAverage(Filter.M).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.MostRuns),
				Link = $"/Course/{course.ID}/{ResultType.MostRuns}",
				Filter = Filter.None,
				Rows = new Lazy<LeaderboardRow[]>(() => course.MostRuns(Filter.None).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString() })
					.ToArray())
			},
			new()
			{
				Title = "Age Grade",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Filter = Filter.None,
				Rows = new Lazy<LeaderboardRow[]>(() => course.TeamPoints(Filter.None).OrderByDescending(p => p.Value.AverageAgeGrade).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.Value.AgeGradePoints), Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.AverageAgeGrade.Display })
					.ToArray())
			},
			new()
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Filter = Filter.None,
				Rows = new Lazy<LeaderboardRow[]>(() => course.TeamPoints(Filter.None).OrderByDescending(p => p.Value.TotalRuns).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.Value.MostRunsPoints), Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.TotalRuns.ToString() })
					.ToArray())
			},
			new()
			{
				Title = "Team Points",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Filter = Filter.None,
				Rows = new Lazy<LeaderboardRow[]>(() => course.TeamPoints(Filter.None).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.TotalPoints.ToString() })
					.ToArray())
			}
		];

	private static Func<LeaderboardTable, bool> GetFilter(LeaderboardResultType type)
	{
		return type switch
		{
			LeaderboardResultType.F => t => t.Filter.Category == Category.F || t.Filter.Category == null && t.ResultType?.Value != ResultType.Team,
			LeaderboardResultType.M => t => t.Filter.Category == Category.M || t.Filter.Category == null && t.ResultType?.Value != ResultType.Team,
			_ => t => t.ResultType?.Value == ResultType.Team
		};
	}
}