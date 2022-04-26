using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Leaders;

public class LeaderboardViewModel : ViewModel
{
	public override string Title => "Leaderboard";

	public LeaderboardResultType LeaderboardResultType { get; }

	public int? Month { get; }
	public IReadOnlyCollection<DateOnly> Months { get; init; }

	private readonly IReadOnlyCollection<Course> _courses;
	private readonly Func<LeaderboardTable, bool> _leaderboardFilter;
	private readonly Filter _resultFilter;


	public LeaderboardViewModel(IReadOnlyCollection<Course> courses, LeaderboardResultType type, byte? month = null)
	{
		var courseList = courses.ToArray();
		_courses = courseList;

		Months = _courses.DistinctMonths();
		Month = month;
		_resultFilter = new Filter { Month = month };

		LeaderboardResultType = type;
		_leaderboardFilter = GetFilter(type);
	}

	private LeaderboardTable OverallTable(string id, ResultType type, Category category, Func<IReadOnlyCollection<LeaderboardRow>> rows)
		=> Config.Competitions.ContainsKey(id)
			? new LeaderboardTable
			{
				Link = $"/Overall/{id}/{Month}",
				Title = Config.Competitions[id],
				ResultType = new FormattedResultType(type),
				Category = category,
				Rows = new Lazy<IReadOnlyCollection<LeaderboardRow>>(rows)
			}
			: null;

	public IReadOnlyCollection<LeaderboardTable> OverallResults
	{
		get
		{
			var vm = new OverallResults(_courses);
			var leaderboards = new List<LeaderboardTable>
			{
				OverallTable("Points/F", ResultType.Fastest, Category.F, () => vm.MostPoints(_resultFilter with { Category = Category.F }).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("Points/M", ResultType.Fastest, Category.M, () => vm.MostPoints(_resultFilter with { Category = Category.M }).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("PointsTop3/F", ResultType.Fastest, Category.F, () => vm.MostPoints(3, _resultFilter with { Category = Category.F }).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("PointsTop3/M", ResultType.Fastest, Category.M, () => vm.MostPoints(3, _resultFilter with { Category = Category.M }).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("AgeGrade", ResultType.BestAverage, null, () => vm.AgeGrade(_resultFilter).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.AgeGrade.Display })
					.ToArray()),

				OverallTable("Miles", ResultType.MostRuns, null, () => vm.MostMiles(_resultFilter).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("Community", ResultType.Community, null, () => vm.CommunityStars(_resultFilter).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Value.ToString() })
					.ToArray()),

				OverallTable("Team", ResultType.Team, null, () => vm.TeamPoints(_resultFilter).Take(3)
					.Select(t => new LeaderboardRow { Rank = t.Rank, Name = t.Value.Team.Display, Link = $"/Team/Index/{t.Value.Team.Value}", Value = t.Value.TotalPoints.ToString() })
					.ToArray())
			};

			return leaderboards.Where(l => l != null).ToArray();
		}
	}

	public IDictionary<Course, LeaderboardTable[]> CourseResults
		=> _courses.ToDictionary(c => c, c => LeaderboardTables(c).Where(t => Config.Features.MultiAttempt ? _leaderboardFilter(t) : t.ResultType.Value == ResultType.Fastest)
			.ToArray());

	private IReadOnlyCollection<LeaderboardTable> LeaderboardTables(Course course)
		=> new List<LeaderboardTable>
		{
			new()
			{
				Title = "Fastest (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Category = Category.F,
				Link = $"/Course/{course.ID}/{course.Distance.Display}/{ResultType.Fastest}/F",
				Rows = new Lazy<IReadOnlyCollection<LeaderboardRow>>(() => course.Fastest(_resultFilter with { Category = Category.F }).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Fastest (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Category = Category.M,
				Link = $"/Course/{course.ID}/{course.Distance.Display}/{ResultType.Fastest}/M",
				Rows = new Lazy<IReadOnlyCollection<LeaderboardRow>>(() => course.Fastest(_resultFilter with { Category = Category.M }).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Best Average (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Category = Category.F,
				Link = $"/Course/{course.ID}/{ResultType.BestAverage}/F",
				Rows = new Lazy<IReadOnlyCollection<LeaderboardRow>>(() => course.BestAverage(_resultFilter with { Category = Category.F }).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Best Average (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Category = Category.M,
				Link = $"/Course/{course.ID}/{ResultType.BestAverage}/M",
				Rows = new Lazy<IReadOnlyCollection<LeaderboardRow>>(() => course.BestAverage(_resultFilter with { Category = Category.M }).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.MostRuns),
				Link = $"/Course/{course.ID}/{ResultType.MostRuns}",
				Rows = new Lazy<IReadOnlyCollection<LeaderboardRow>>(() => course.MostRuns(_resultFilter).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString() })
					.ToArray())
			},
			new()
			{
				Title = "Age Grade",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Rows = new Lazy<IReadOnlyCollection<LeaderboardRow>>(() => course.TeamPoints(_resultFilter).OrderByDescending(p => p.Value.AverageAgeGrade).Take(3)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.Value.AgeGradePoints), Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.AverageAgeGrade.Display })
					.ToArray())
			},
			new()
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Rows = new Lazy<IReadOnlyCollection<LeaderboardRow>>(() => course.TeamPoints(_resultFilter).OrderByDescending(p => p.Value.TotalRuns).Take(3)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.Value.MostRunsPoints), Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.TotalRuns.ToString() })
					.ToArray())
			},
			new()
			{
				Title = "Team Points",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Rows = new Lazy<IReadOnlyCollection<LeaderboardRow>>(() => course.TeamPoints(_resultFilter).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.TotalPoints.ToString() })
					.ToArray())
			}
		};

	private static Func<LeaderboardTable, bool> GetFilter(LeaderboardResultType type)
	{
		return type switch
		{
			LeaderboardResultType.F => t => t.Category == Category.F || t.Category == null && t.ResultType?.Value != ResultType.Team,
			LeaderboardResultType.M => t => t.Category == Category.M || t.Category == null && t.ResultType?.Value != ResultType.Team,
			_ => t => t.ResultType?.Value == ResultType.Team
		};
	}
}