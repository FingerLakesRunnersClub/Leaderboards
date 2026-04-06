using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Athlete = FLRC.Leaderboards.Model.Athlete;
using Course = FLRC.Leaderboards.Model.Course;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class StatisticsController(IIterationManager iterationManager) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index()
	{
		var iteration = await iterationManager.ActiveIteration();
		var stats = await GetStatistics(iteration);
		var vm = new ViewModel<StatisticsViewModel>("Statistics", stats);
		return View(vm);
	}

	private async Task<StatisticsViewModel> GetStatistics(Iteration iteration)
	{
		var officialCourses = iteration.OfficialChallenge.Courses.OrderBy(c => new Distance(c.DistanceDisplay).Meters);
		var otherCourses = iteration.Races.SelectMany(r => r.Courses).Except(officialCourses).OrderBy(c => new Distance(c.DistanceDisplay).Meters);
		var courses = officialCourses.Concat(otherCourses).ToArray();

		var courseStats = courses.ToDictionary(c => c, c => c.Results.Where(r => DateOnly.FromDateTime(r.StartTime) >= iteration.StartDate && DateOnly.FromDateTime(r.StartTime) <= iteration.EndDate).ToArray().Statistics());
		var athletes = courseStats.SelectMany(stats => stats.Key.Results.Where(r => DateOnly.FromDateTime(r.StartTime) >= iteration.StartDate && DateOnly.FromDateTime(r.StartTime) <= iteration.EndDate).Select(r => r.Athlete)).Distinct().ToArray();

		var history = courses.SelectMany(c => c.Results)
			.Where(r => DateOnly.FromDateTime(r.StartTime) >= iteration.StartDate && DateOnly.FromDateTime(r.StartTime) <= iteration.EndDate)
			.GroupBy(r => new Date(r.StartTime).Week)
			.OrderByDescending(r => r.Key)
			.ToDictionary(g => g.Key, WeeklyStatistics);

		return new StatisticsViewModel
		{
			Courses = courseStats,
			History = history,
			Total = GetTotals(athletes, courseStats)
		};
	}

	private static Statistics GetTotals(Athlete[] athletes, IDictionary<Course, Statistics> courseStats)
		=> new()
		{
			Participants = new Dictionary<string, int>
			{
				{ string.Empty, athletes.Length },
				{ Category.F.Display, athletes.Count(a => a.Category == 'F') },
				{ Category.M.Display, athletes.Count(a => a.Category == 'M') }
			},
			Runs = new Dictionary<string, int>
			{
				{ string.Empty, courseStats.Sum(stats => stats.Value.Runs[string.Empty]) },
				{ Category.F.Display, courseStats.Sum(stats => stats.Value.Runs[Category.F.Display]) },
				{ Category.M.Display, courseStats.Sum(stats => stats.Value.Runs[Category.M.Display]) }
			},
			Miles = new Dictionary<string, double>
			{
				{ string.Empty, courseStats.Sum(stats => stats.Value.Miles[string.Empty]) },
				{ Category.F.Display, courseStats.Sum(stats => stats.Value.Miles[Category.F.Display]) },
				{ Category.M.Display, courseStats.Sum(stats => stats.Value.Miles[Category.M.Display]) }
			},
			Average = new Dictionary<string, double>
			{
				{
					string.Empty,
					(double) courseStats.Sum(stats => stats.Value.Runs[string.Empty]) / athletes.Length
				},
				{
					Category.F.Display,
					(double) courseStats.Sum(stats => stats.Value.Runs[Category.F.Display]) /
					athletes.Count(a => a.Category == 'F')
				},
				{
					Category.M.Display,
					(double) courseStats.Sum(stats => stats.Value.Runs[Category.M.Display]) /
					athletes.Count(a => a.Category == 'M')
				}
			}
		};

	private static Statistics WeeklyStatistics(IEnumerable<Result> results)
	{
		var resultList = results.ToArray();
		var athletes = resultList.Select(r => r.Athlete).Distinct().ToArray();
		var fResults = resultList.Where(r => r.Athlete.Category == 'F').ToArray();
		var mResults = resultList.Where(r => r.Athlete.Category == 'M').ToArray();

		var fAthletes = athletes.Where(a => a.Category == 'F').ToArray();
		var mAthletes = athletes.Where(a => a.Category == 'M').ToArray();
		return new Statistics
		{
			Participants = new Dictionary<string, int>
			{
				{ string.Empty, athletes.Length },
				{ Category.F.Display, fAthletes.Length },
				{ Category.M.Display, mAthletes.Length }
			},
			Runs = new Dictionary<string, int>
			{
				{ string.Empty, resultList.Length },
				{ Category.F.Display, fResults.Length },
				{ Category.M.Display, mResults.Length }
			},
			Miles = new Dictionary<string, double>
			{
				{
					string.Empty,
					resultList.Sum(r => new Distance(r.Course.DistanceDisplay).Miles)
				},
				{
					Category.F.Display,
					fResults.Sum(r => new Distance(r.Course.DistanceDisplay).Miles)
				},
				{
					Category.M.Display,
					mResults.Sum(r => new Distance(r.Course.DistanceDisplay).Miles)
				}
			},
			Average = new Dictionary<string, double>
			{
				{ string.Empty, (double) resultList.Length / athletes.Length },
				{ Category.F.Display, (double) fResults.Length / fAthletes.Length },
				{ Category.M.Display, (double) mResults.Length / mAthletes.Length }
			}
		};
	}
}