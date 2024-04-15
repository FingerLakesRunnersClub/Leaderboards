using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class StatisticsController : Controller
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
		var results = (await _dataService.GetAllResults()).ToArray();

		var courseStats = results.ToDictionary(c => c, c => c.Statistics());
		var athletes = courseStats.SelectMany(stats => stats.Key.Results.Select(r => r.Athlete)).Distinct()
			.ToArray();

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

	private static Statistics weeklyStatistics(IEnumerable<Result> results)
	{
		var resultList = results.ToArray();
		var athletes = resultList.Select(r => r.Athlete).Distinct().ToArray();
		var fResults = resultList.Where(r => r.Athlete.Category == Category.F).ToArray();
		var mResults = resultList.Where(r => r.Athlete.Category == Category.M).ToArray();

		var fAthletes = athletes.Where(a => a.Category == Category.F).ToArray();
		var mAthletes = athletes.Where(a => a.Category == Category.M).ToArray();
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
					resultList.Sum(r => r.Course.Distance.Miles)
				},
				{
					Category.F.Display,
					fResults.Sum(r => r.Course.Distance.Miles)
				},
				{
					Category.M.Display,
					mResults.Sum(r => r.Course.Distance.Miles)
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