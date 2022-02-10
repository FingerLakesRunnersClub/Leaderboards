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

	private readonly IEnumerable<Course> _courses;
	private readonly Func<LeaderboardTable, bool> _filter;

	public LeaderboardViewModel(IEnumerable<Course> courses, LeaderboardResultType type)
	{
		var courseList = courses.ToList();
		_courses = courseList;
		_filter = GetFilter(type);
		LeaderboardResultType = type;
	}

	private LeaderboardTable OverallTable(string id, ResultType type, Category category, Func<IEnumerable<LeaderboardRow>> rows)
		=> Config.Competitions.ContainsKey(id)
			? new LeaderboardTable
			{
				Link = $"/Overall/{id}",
				Title = Config.Competitions[id],
				ResultType = new FormattedResultType(type),
				Category = category,
				Rows = new Lazy<IEnumerable<LeaderboardRow>>(rows)
			}
			: null;

	public IEnumerable<LeaderboardTable> OverallResults
	{
		get
		{
			var vm = new OverallResults(_courses);
			var leaderboards = new List<LeaderboardTable>
			{
				OverallTable("Points/F", ResultType.Fastest, Category.F, () => vm.MostPoints(Category.F).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })),

				OverallTable("Points/M", ResultType.Fastest, Category.M, () => vm.MostPoints(Category.M).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })),

				OverallTable("PointsTop3/F", ResultType.Fastest, Category.F, () => vm.MostPoints(3, Category.F).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })),

				OverallTable("PointsTop3/M", ResultType.Fastest, Category.M, () => vm.MostPoints(3, Category.M).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })),

				OverallTable("AgeGrade", ResultType.BestAverage, null, () => vm.AgeGrade().Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.AgeGrade.Display })),

				OverallTable("Miles", ResultType.MostRuns, null, () => vm.MostMiles().Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString("F1") })),

				OverallTable("Team", ResultType.Team, null, () => vm.TeamPoints().Take(3)
					.Select(t => new LeaderboardRow { Rank = t.Rank, Name = t.Team.Display, Link = $"/Team/Index/{t.Team.Value}", Value = t.TotalPoints.ToString() }))
			};

			return leaderboards.Where(l => l != null);
		}
	}

	public IDictionary<Course, IEnumerable<LeaderboardTable>> CourseResults
		=> _courses.ToDictionary(c => c, c => LeaderboardTables(c).Where(t => Config.Features.MultiAttempt ? _filter(t) : t.ResultType.Value == ResultType.Fastest));

	private static IEnumerable<LeaderboardTable> LeaderboardTables(Course course)
		=> new List<LeaderboardTable>
		{
			new()
			{
				Title = "Fastest (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Category = Category.F,
				Link = $"/Course/{course.ID}/{ResultType.Fastest}/F",
				Rows = new Lazy<IEnumerable<LeaderboardRow>>(() => course.Fastest(Category.F).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display }))
			},
			new()
			{
				Title = "Fastest (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Category = Category.M,
				Link = $"/Course/{course.ID}/{ResultType.Fastest}/M",
				Rows = new Lazy<IEnumerable<LeaderboardRow>>(() => course.Fastest(Category.M).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display }))
			},
			new()
			{
				Title = "Best Average (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Category = Category.F,
				Link = $"/Course/{course.ID}/{ResultType.BestAverage}/F",
				Rows = new Lazy<IEnumerable<LeaderboardRow>>(() => course.BestAverage(Category.F).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display }))
			},
			new()
			{
				Title = "Best Average (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Category = Category.M,
				Link = $"/Course/{course.ID}/{ResultType.BestAverage}/M",
				Rows = new Lazy<IEnumerable<LeaderboardRow>>(() => course.BestAverage(Category.M).Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display }))
			},
			new()
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.MostRuns),
				Link = $"/Course/{course.ID}/{ResultType.MostRuns}",
				Rows = new Lazy<IEnumerable<LeaderboardRow>>(() => course.MostRuns().Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString() }))
			},
			new()
			{
				Title = "Age Grade",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Rows = new Lazy<IEnumerable<LeaderboardRow>>(() => course.TeamPoints().OrderByDescending(p => p.AverageAgeGrade).Take(3)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.AgeGradePoints), Name = r.Team.Display, Link = $"/Team/Index/{r.Team.Value}", Value = r.AverageAgeGrade.Display }))
			},
			new()
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Rows = new Lazy<IEnumerable<LeaderboardRow>>(() => course.TeamPoints().OrderByDescending(p => p.TotalRuns).Take(3)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.MostRunsPoints), Name = r.Team.Display, Link = $"/Team/Index/{r.Team.Value}", Value = r.TotalRuns.ToString() }))
			},
			new()
			{
				Title = "Total Points",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Course/{course.ID}/{ResultType.Team}",
				Rows = new Lazy<IEnumerable<LeaderboardRow>>(() => course.TeamPoints().Take(3)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Name = r.Team.Display, Link = $"/Team/Index/{r.Team.Value}", Value = r.TotalPoints.ToString() }))
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