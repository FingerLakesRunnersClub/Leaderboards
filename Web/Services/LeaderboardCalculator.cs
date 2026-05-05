using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Leaders;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.ViewModels;
using Course = FLRC.Leaderboards.Model.Course;
using LeaderboardTable = FLRC.Leaderboards.Web.ViewModels.LeaderboardTable;

namespace FLRC.Leaderboards.Web.Services;

public sealed class LeaderboardCalculator(ICommunityStarCalculator starCalculator, IConfig config, Iteration iteration, LeaderboardResultType type, byte tableSize)
{
	private readonly Func<LeaderboardTable, bool> _leaderboardFilter = GetFilter(type);

	public Leaderboard GetLeaderboard(LeaderboardResultType type)
		=> new()
		{
			Config = config,
			LeaderboardResultType = type,
			OverallResults = OverallResults(),
			OfficialCourses = OfficialCourses(),
			OtherCourses = OtherCourses()
		};

	private LeaderboardTable[] OverallResults()
	{
		var vm = new OverallResultsCalculator(starCalculator, iteration);
		var leaderboards = new List<LeaderboardTable>
		{
			OverallTable("Points/F", ResultType.Fastest, new Filter(Category.F), () => vm.MostPoints(new Filter(Category.F)).Take(tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("Points/M", ResultType.Fastest, new Filter(Category.M), () => vm.MostPoints(new Filter(Category.M)).Take(tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("PointsTop3/F", ResultType.Fastest, new Filter(Category.F), () => vm.MostPoints(tableSize, new Filter(Category.F)).Take(tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("PointsTop3/M", ResultType.Fastest, new Filter(Category.M), () => vm.MostPoints(tableSize, new Filter(Category.M)).Take(tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("AgeGrade", ResultType.BestAverage, new Filter(), () => vm.AgeGrade().Take(tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.AgeGrade.Display })
				.ToArray()),

			OverallTable("Miles", ResultType.MostRuns, new Filter(), () => vm.MostMiles().Take(tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("Courses", ResultType.MostCourses, new Filter(), () => vm.MostCourses().Take(tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString() })
				.ToArray()),

			OverallTable("Community", ResultType.Community, new Filter(), () => vm.Community().Take(tableSize)
				.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
				.ToArray()),

			OverallTable("Team", ResultType.Team, new Filter(), () => vm.TeamPoints().Take(tableSize)
				.Select(t => new LeaderboardRow { Rank = t.Rank, Name = t.Value.Team.Display, Link = $"/Team/Index/{t.Value.Team.Value}", Value = t.Value.TotalPoints.ToString() })
				.ToArray())
		};

		return leaderboards.Where(l => l != null).ToArray();
	}

	private LeaderboardTable OverallTable(string id, ResultType type, Filter filter, Func<LeaderboardRow[]> rows)
		=> config.Competitions.TryGetValue(id, out var title)
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
		=> CourseResults(iteration.OfficialChallenge?.Courses.ToArray() ?? []);

	private Dictionary<Course, LeaderboardTable[]> OtherCourses()
		=> CourseResults(iteration.Races.SelectMany(r => r.Courses).Except(iteration.OfficialChallenge?.Courses ?? []).ToArray());

	private Dictionary<Course, LeaderboardTable[]> CourseResults(Course[] courses)
		=> courses.OrderBy(c => new Distance(c.DistanceDisplay).Meters)
			.ToDictionary(c => c, GetLeaderboardTables);

	private LeaderboardTable[] GetLeaderboardTables(Course c)
		=> AllCourseTables(c)
			.Where(t => t.Rows.Value.Length > 0 && FilterMatches(t))
			.ToArray();

	private bool FilterMatches(LeaderboardTable t)
		=> config.Features.MultiAttemptCompetitions
			? _leaderboardFilter(t)
			: t.ResultType.Value is ResultType.Fastest or ResultType.Farthest;

	private LeaderboardTable[] AllCourseTables(Course course)
	{
		var results = course.Results.For(iteration);
		return
		[
			new LeaderboardTable
			{
				Title = "Fastest (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Filter = new Filter(Category.F),
				Link = $"/Results/Fastest/{course.ID}?c=F",
				Rows = new Lazy<LeaderboardRow[]>(() => results.Fastest(new Filter(Category.F)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Fastest (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Fastest),
				Filter = new Filter(Category.M),
				Link = $"/Results/Fastest/{course.ID}?c=M",
				Rows = new Lazy<LeaderboardRow[]>(() => results.Fastest(new Filter(Category.M)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Best Average (F)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Filter = new Filter(Category.F),
				Link = $"/Results/BestAverage/{course.ID}?c=F",
				Rows = new Lazy<LeaderboardRow[]>(() => results.BestAverage(new Filter(Category.F)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Best Average (M)",
				Course = course,
				ResultType = new FormattedResultType(ResultType.BestAverage),
				Filter = new Filter(Category.M),
				Link = $"/Results/BestAverage/{course.ID}?c=M",
				Rows = new Lazy<LeaderboardRow[]>(() => results.BestAverage(new Filter(Category.M)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.MostRuns),
				Link = $"/Results/MostRuns/{course.ID}",
				Filter = new Filter(),
				Rows = new Lazy<LeaderboardRow[]>(() => results.MostRuns().Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.ToString() })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Age Grade",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Results/Team/{course.ID}",
				Filter = new Filter(),
				Rows = new Lazy<LeaderboardRow[]>(() => results.TeamPoints(iteration, new Filter()).OrderByDescending(p => p.Value.AverageAgeGrade).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.Value.AgeGradePoints), Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.AverageAgeGrade.Display })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Most Runs",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Results/Team/{course.ID}",
				Filter = new Filter(),
				Rows = new Lazy<LeaderboardRow[]>(() => results.TeamPoints(iteration, new Filter()).OrderByDescending(p => p.Value.TotalRuns).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = new Rank(r.Value.MostRunsPoints), Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.TotalRuns.ToString() })
					.ToArray())
			},
			new LeaderboardTable
			{
				Title = "Team Points",
				Course = course,
				ResultType = new FormattedResultType(ResultType.Team),
				Link = $"/Results/Team/{course.ID}",
				Filter = new Filter(),
				Rows = new Lazy<LeaderboardRow[]>(() => results.TeamPoints(iteration, new Filter()).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Name = r.Value.Team.Display, Link = $"/Team/Index/{r.Value.Team.Value}", Value = r.Value.TotalPoints.ToString() })
					.ToArray())
			}
		];
	}

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