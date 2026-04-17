using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Leaders;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;
using Course = FLRC.Leaderboards.Model.Course;
using LeaderboardTable = FLRC.Leaderboards.Web.ViewModels.LeaderboardTable;

namespace FLRC.Leaderboards.Web.Services;

public sealed class LeaderboardCalculator
{
	private readonly Iteration _iteration;
	private readonly byte _tableSize;
	private readonly Func<LeaderboardTable, bool> _leaderboardFilter;
	private readonly IConfig _config;

	public LeaderboardCalculator(IConfig config, Iteration iteration, LeaderboardResultType type, byte tableSize)
	{
		_config = config;
		_iteration = iteration;
		_leaderboardFilter = GetFilter(type);
		_tableSize = tableSize;
	}

	public Leaderboard GetLeaderboard(LeaderboardResultType type)
		=> new()
		{
			Config = _config,
			LeaderboardResultType = type,
			OverallResults = OverallResults(),
			OfficialCourses = OfficialCourses(),
			OtherCourses = OtherCourses()
		};

	private LeaderboardTable[] OverallResults()
	{
		var vm = new OverallResultsCalculator(_iteration);
		var leaderboards = new List<LeaderboardTable>
		{
			OverallTable("Points/F", ResultType.Fastest, new Filter(Category.F), () => vm.MostPoints(new Filter(Category.F)).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("Points/M", ResultType.Fastest, new Filter(Category.M), () => vm.MostPoints(new Filter(Category.M)).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("PointsTop3/F", ResultType.Fastest, new Filter(Category.F), () => vm.MostPoints(_tableSize, new Filter(Category.F)).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("PointsTop3/M", ResultType.Fastest, new Filter(Category.M), () => vm.MostPoints(_tableSize, new Filter(Category.M)).Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("AgeGrade", ResultType.BestAverage, new Filter(), () => vm.AgeGrade().Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.AgeGrade.Display })
				.ToArray()),

			OverallTable("Miles", ResultType.MostRuns, new Filter(), () => vm.MostMiles().Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("Courses", ResultType.MostCourses, new Filter(), () => vm.MostCourses().Take(_tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString() })
				.ToArray())
		};

		return leaderboards.Where(l => l != null).ToArray();
	}

	private LeaderboardTable OverallTable(string id, ResultType type, Filter filter, Func<LeaderboardRow[]> rows)
		=> _config.Competitions.TryGetValue(id, out var title)
			? new LeaderboardTable
			{
				Link = $"/Overall/{id}",
				Title = title,
				ResultType = new FormattedResultType(type),
				Filter = filter ?? new Filter(),
				Rows = new Lazy<LeaderboardRow[]>(rows)
			}
			: null;

	private Dictionary<Course, LeaderboardTable[]> OfficialCourses()
		=> CourseResults(_iteration.OfficialChallenge?.Courses.ToArray() ?? []);

	private Dictionary<Course, LeaderboardTable[]> OtherCourses()
		=> CourseResults(_iteration.Races.SelectMany(r => r.Courses).Except(_iteration.OfficialChallenge?.Courses ?? []).ToArray());

	private Dictionary<Course, LeaderboardTable[]> CourseResults(Course[] courses)
		=> courses.OrderBy(c => new Distance(c.DistanceDisplay).Meters)
			.ToDictionary(c => c, c => CourseTables(c, _tableSize)
				.Where(t => _config.Features.MultiAttemptCompetitions
					? _leaderboardFilter(t)
					: t.ResultType.Value is ResultType.Fastest or ResultType.Farthest)
			.Where(t => t.Rows.Value.Length > 0)
			.ToArray());

	private LeaderboardTable[] CourseTables(Course course, byte tableSize)
		=> [
			new()
			{
				Title = "Fastest (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Filter = new Filter(Category.F),
				Link = $"/Results/Fastest/{course.ID}?c=F",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Results.For(_iteration).Fastest(new Filter(Category.F)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Fastest (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Filter = new Filter(Category.M),
				Link = $"/Results/Fastest/{course.ID}?c=M",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Results.For(_iteration).Fastest(new Filter(Category.M)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Best Average (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Filter = new Filter(Category.F),
				Link = $"/Results/BestAverage/{course.ID}?c=F",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Results.For(_iteration).BestAverage(new Filter(Category.F)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Best Average (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Filter = new Filter(Category.M),
				Link = $"/Results/BestAverage/{course.ID}?c=M",
				Rows = new Lazy<LeaderboardRow[]>(() => course.Results.For(_iteration).BestAverage(new Filter(Category.M)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new()
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.MostRuns),
				Link = $"/Results/MostRuns/{course.ID}",
				Filter = new Filter(),
				Rows = new Lazy<LeaderboardRow[]>(() => course.Results.For(_iteration).MostRuns().Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString() })
					.ToArray())
			},
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