using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class StatisticsController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public StatisticsController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index() => View(await GetStatistics());

	private async Task<StatisticsViewModel> GetStatistics()
	{
		var results = (await _dataService.GetAllResults()).ToList();

		var courseStats = results.ToDictionary(c => c, c => c.Statistics());
		var athletes = courseStats.SelectMany(stats => stats.Key.Results.Select(r => r.Athlete)).Distinct()
			.ToList();

		var history = results.SelectMany(c => c.Results).GroupBy(r => r.StartTime.Week)
			.OrderByDescending(r => r.Key)
			.ToDictionary(g => g.Key, weeklyStatistics);

		return new StatisticsViewModel
		{
			Config = _config,
			Courses = courseStats,
			History = history,
			Total = GetTotals(athletes, courseStats)
		};
	}

	private static Statistics GetTotals(IReadOnlyCollection<Athlete> athletes, IDictionary<Course, Statistics> courseStats)
		=> new()
		{
			Participants = new Dictionary<string, int>
			{
				{ string.Empty, athletes.Count },
				{ Category.F.Display, athletes.Count(a => a.Category == Category.F) },
				{ Category.M.Display, athletes.Count(a => a.Category == Category.M) }
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
					(double) courseStats.Sum(stats => stats.Value.Runs[string.Empty]) / athletes.Count
				},
				{
					Category.F.Display,
					(double) courseStats.Sum(stats => stats.Value.Runs[Category.F.Display]) /
					athletes.Count(a => a.Category == Category.F)
				},
				{
					Category.M.Display,
					(double) courseStats.Sum(stats => stats.Value.Runs[Category.M.Display]) /
					athletes.Count(a => a.Category == Category.M)
				}
			}
		};

	private Statistics weeklyStatistics(IEnumerable<Result> results)
	{
		var resultList = results.ToList();
		var athletes = resultList.Select(r => r.Athlete).Distinct().ToList();
		var fResults = resultList.Where(r => r.Athlete.Category == Category.F).ToList();
		var mResults = resultList.Where(r => r.Athlete.Category == Category.M).ToList();

		var fAthletes = athletes.Where(a => a.Category == Category.F).ToList();
		var mAthletes = athletes.Where(a => a.Category == Category.M).ToList();
		return new Statistics
		{
			Participants = new Dictionary<string, int>
			{
				{ string.Empty, athletes.Count },
				{ Category.F.Display, fAthletes.Count },
				{ Category.M.Display, mAthletes.Count }
			},
			Runs = new Dictionary<string, int>
			{
				{ string.Empty, resultList.Count },
				{ Category.F.Display, fResults.Count },
				{ Category.M.Display, mResults.Count }
			},
			Miles = new Dictionary<string, double>
			{
				{
					string.Empty,
					resultList.Sum(r => r.Course.Distance.Miles)
				},
				{
					Category.F.Display,
					fResults.Sum(r => r.Course.Distance.Miles)
				},
				{
					Category.M.Display,
					mResults.Sum(r => r.Course.Distance.Miles)
				},
			},
			Average = new Dictionary<string, double>
			{
				{ string.Empty, (double) resultList.Count / athletes.Count },
				{ Category.F.Display, (double) fResults.Count / fAthletes.Count },
				{ Category.M.Display, (double) mResults.Count / mAthletes.Count }
			}
		};
	}
}