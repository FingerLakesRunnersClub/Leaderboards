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

public sealed class LeaderboardCalculator(IOverallResultsCalculator overall, IConfig config) : ILeaderboardCalculator
{
	public Leaderboard GetLeaderboard(Iteration iteration, LeaderboardResultType type, byte tableSize)
		=> new()
		{
			Config = config,
			LeaderboardResultType = type,
			OverallResults = OverallResults(iteration, tableSize),
			OfficialCourses = OfficialCourses(iteration, type, tableSize),
			OtherCourses = OtherCourses(iteration, type, tableSize)
		};

	private LeaderboardTable[] OverallResults(Iteration iteration, byte tableSize)
		=> new List<LeaderboardTable>
			{
				OverallTable("Points/F", ResultType.Fastest, new Filter(Category.F), () => overall.MostPoints(iteration, new Filter(Category.F)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("Points/M", ResultType.Fastest, new Filter(Category.M), () => overall.MostPoints(iteration, new Filter(Category.M)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("PointsTop3/F", ResultType.Fastest, new Filter(Category.F), () => overall.MostPoints(iteration, tableSize, new Filter(Category.F)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("PointsTop3/M", ResultType.Fastest, new Filter(Category.M), () => overall.MostPoints(iteration, tableSize, new Filter(Category.M)).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("AgeGrade", ResultType.BestAverage, new Filter(), () => overall.AgeGrade(iteration).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.AgeGrade.Display })
					.ToArray()),

				OverallTable("Miles", ResultType.MostRuns, new Filter(), () => overall.MostMiles(iteration).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("Courses", ResultType.MostCourses, new Filter(), () => overall.MostCourses(iteration).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("Community", ResultType.Community, new Filter(), () => overall.Community(iteration).Take(tableSize)
					.Select(r => new LeaderboardRow { Rank = r.Rank, Link = $"/Athlete/Index/{r.Result.Athlete.ID}", Name = r.Result.Athlete.Name, Value = r.Value.Display })
					.ToArray()),

				OverallTable("Team", ResultType.Team, new Filter(), () => overall.TeamPoints(iteration).Take(tableSize)
					.Select(t => new LeaderboardRow { Rank = t.Rank, Name = t.Value.Team.Display, Link = $"/Team/Index/{t.Value.Team.Value}", Value = t.Value.TotalPoints.ToString() })
					.ToArray())
			}
			.Where(l => l != null)
			.ToArray();

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

	private Dictionary<Course, LeaderboardTable[]> OfficialCourses(Iteration iteration, LeaderboardResultType type, byte tableSize)
		=> CourseResults(iteration, iteration.OfficialChallenge?.Courses.ToArray() ?? [], type, tableSize);

	private Dictionary<Course, LeaderboardTable[]> OtherCourses(Iteration iteration, LeaderboardResultType type, byte tableSize)
		=> CourseResults(iteration, iteration.Races.SelectMany(r => r.Courses).Except(iteration.OfficialChallenge?.Courses ?? []).ToArray(), type, tableSize);

	private Dictionary<Course, LeaderboardTable[]> CourseResults(Iteration iteration, Course[] courses, LeaderboardResultType type, byte tableSize)
		=> courses.OrderBy(c => new Distance(c.DistanceDisplay).Meters)
			.ToDictionary(c => c, c => GetLeaderboardTables(iteration, c, type, tableSize));

	private LeaderboardTable[] GetLeaderboardTables(Iteration iteration, Course course, LeaderboardResultType type, byte tableSize)
		=> AllCourseTables(iteration, course, tableSize)
			.Where(t => t.Rows.Value.Length > 0 && FilterMatches(t, type))
			.ToArray();

	private bool FilterMatches(LeaderboardTable table, LeaderboardResultType type)
		=> config.Features.MultiAttemptCompetitions
			? GetFilter(type)(table)
			: table.ResultType.Value is ResultType.Fastest or ResultType.Farthest;

	private static LeaderboardTable[] AllCourseTables(Iteration iteration, Course course, byte tableSize)
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