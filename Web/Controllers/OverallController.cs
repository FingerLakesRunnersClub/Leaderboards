using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class OverallController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public OverallController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Points(string id, byte? month = null)
	{
		var filter = new Filter { Category = Category.Parse(id), Month = month };
		return View(await GetResults(_config.Competitions[$"Points/{id}"], overall => overall.MostPoints(filter), month));
	}

	public async Task<ViewResult> PointsTop3(string id, byte? month = null)
	{
		var filter = new Filter { Category = Category.Parse(id), Month = month };
		return View("Points", await GetResults(_config.Competitions[$"PointsTop3/{id}"], overall => overall.MostPoints(3, filter), month));
	}

	public async Task<ViewResult> Miles(byte? month = null)
	{
		var filter = new Filter { Month = month };
		return View(await GetResults(_config.Competitions["Miles"], overall => overall.MostMiles(filter), month));
	}

	public async Task<ViewResult> AgeGrade(byte? month = null)
	{
		var filter = new Filter { Month = month };
		return View(await GetResults(_config.Competitions["AgeGrade"], overall => overall.AgeGrade(filter), month));
	}

	public async Task<ViewResult> Community(byte? month = null)
	{
		var filter = new Filter { Month = month };
		return View(await GetResults(_config.Competitions["Community"], overall => overall.CommunityStars(filter), month));
	}

	public async Task<ViewResult> Team(byte? month = null)
	{
		var filter = new Filter { Month = month };
		return View(await GetTeamResults(_config.Competitions["Team"], overall => overall.TeamPoints(filter), month));
	}

	private async Task<OverallResultsViewModel<T>> GetResults<T>(string title, Func<OverallResults, RankedList<T>> results, byte? month)
	{
		var allResults = await _dataService.GetAllResults();
		var overall = new OverallResults(allResults);
		var rankedResults = results(overall);

		return new OverallResultsViewModel<T>
		{
			Config = _config,
			ResultType = title,
			Month = month,
			Months = allResults.DistinctMonths().ToArray(),
			RankedResults = rankedResults
		};
	}

	private async Task<OverallResultsViewModel<TeamResults>> GetTeamResults(string title, Func<OverallResults, RankedList<TeamResults>> results, byte? month)
	{
		var allResults = await _dataService.GetAllResults();
		var overall = new OverallResults(allResults);
		return new OverallResultsViewModel<TeamResults>
		{
			Config = _config,
			ResultType = title,
			Month = month,
			Months = allResults.DistinctMonths().ToArray(),
			RankedResults = results(overall)
		};
	}
}