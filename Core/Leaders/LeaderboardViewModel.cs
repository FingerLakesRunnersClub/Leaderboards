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
	private readonly Func<LeaderboardTable, bool> _leaderboardFilter;

	public LeaderboardViewModel(Course[] courses, LeaderboardResultType type)
	{
		_courses = courses;
		LeaderboardResultType = type;
		_leaderboardFilter = GetFilter(type);
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
			OverallTable("Points/F", ResultType.Fastest, Filter.F, () => vm.MostPoints(Filter.F).Take(3)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("Points/M", ResultType.Fastest, Filter.M, () => vm.MostPoints(Filter.M).Take(3)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("PointsTop3/F", ResultType.Fastest, Filter.F, () => vm.MostPoints(3, Filter.F).Take(3)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("PointsTop3/M", ResultType.Fastest, Filter.M, () => vm.MostPoints(3, Filter.M).Take(3)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("AgeGrade", ResultType.BestAverage, Filter.None, () => vm.AgeGrade(Filter.None).Take(3)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.AgeGrade.Display })
				.ToArray()),

			OverallTable("Miles", ResultType.MostRuns, Filter.None, () => vm.MostMiles(Filter.None).Take(3)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("Community", ResultType.Community, Filter.None, () => vm.CommunityStars(Filter.None).Take(3)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Value.ToString() })
				.ToArray()),

			OverallTable("Team", ResultType.Team, Filter.None, () => vm.TeamPoints(Filter.None).Take(3)
				.Select(t => new LeaderboardRow { Rank = t.Rank, Name = t.Value.Team.Display, Link = $"/Team/Index/{t.Value.Team.Value}", Value = t.Value.TotalPoints.ToString() })
				.ToArray())
		};

		return leaderboards.Where(l => l != null).ToArray();
	}

	public IDictionary<Course, LeaderboardTable[]> CourseResults
		=> _courses.ToDictionary(c => c, c => LeaderboardTables(c).Where(t => Config.Features.MultiAttempt ? _leaderboardFilter(t) : t.ResultType.Value == ResultType.Fastest)
			.ToArray());

	private static LeaderboardTable[] LeaderboardTables(Course course)
		=> [
			new LeaderboardTable
			{
				Title = "Fastest (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Filter = Filter.F,
				Link = $"/Course/{course.ID}/{course.ShortName}/{ResultType.Fastest}/F",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Fastest(Filter.F).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Fastest (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Filter = Filter.M,
				Link = $"/Course/{course.ID}/{course.ShortName}/{ResultType.Fastest}/M",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Fastest(Filter.M).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Best Average (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Filter = Filter.F,
				Link = $"/Course/{course.ID}/{ResultType.BestAverage}/F",
				Rows = new Lazy<LeaderboardRow[]>(() => course.BestAverage(Filter.F).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Best Average (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Filter = Filter.M,
				Link = $"/Course/{course.ID}/{ResultType.BestAverage}/M",
				Rows = new Lazy<LeaderboardRow[]>(() => course.BestAverage(Filter.M).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.MostRuns),
				Link = $"/Course/{course.ID}/{ResultType.MostRuns}",
				Filter = Filter.None,
				Rows = new Lazy<LeaderboardRow[]>(() => course.MostRuns(Filter.None).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString() })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Age Grade",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Filter = Filter.None,
				Rows = new Lazy<LeaderboardRow[]>(() => course.TeamPoints(Filter.None).OrderByDescending(p => p.Value.AverageAgeGrade).Take(3)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.Value.AgeGradePoints), Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.AverageAgeGrade.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Filter = Filter.None,
				Rows = new Lazy<LeaderboardRow[]>(() => course.TeamPoints(Filter.None).OrderByDescending(p => p.Value.TotalRuns).Take(3)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.Value.MostRunsPoints), Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.TotalRuns.ToString() })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Team Points",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Filter = Filter.None,
				Rows = new Lazy<LeaderboardRow[]>(() => course.TeamPoints(Filter.None).Take(3)
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