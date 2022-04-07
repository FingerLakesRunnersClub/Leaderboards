using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Ranking;
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

	public async Task<ViewResult> Points(string id)
	{
		var category = Category.Parse(id);
		return View(await GetResults(_config.Competitions[$"Points/{id}"], vm => vm.MostPoints(category)));
	}

	public async Task<ViewResult> PointsTop3(string id)
	{
		var category = Category.Parse(id);
		return View("Points", await GetResults(_config.Competitions[$"PointsTop3/{id}"], vm => vm.MostPoints(3, category)));
	}

	public async Task<ViewResult> Miles()
		=> View(await GetResults(_config.Competitions["Miles"], vm => vm.MostMiles()));

	public async Task<ViewResult> AgeGrade()
		=> View(await GetResults(_config.Competitions["AgeGrade"], vm => vm.AgeGrade()));

	public async Task<ViewResult> Team()
		=> View(await GetTeamResults(_config.Competitions["Team"], vm => vm.TeamPoints()));

	private async Task<OverallResultsViewModel<T>> GetResults<T>(string title, Func<OverallResults, RankedList<T>> results)
	{
		var allResults = await _dataService.GetAllResults();
		var overall = new OverallResults(allResults);
		var rankedResults = results(overall);

		return new OverallResultsViewModel<T>
		{
			Config = _config,
			ResultType = title,
			RankedResults = rankedResults
		};
	}

	private async Task<OverallTeamResultsViewModel> GetTeamResults(string title, Func<OverallResults, IReadOnlyCollection<TeamResults>> results)
	{
		var allResults = await _dataService.GetAllResults();
		var overall = new OverallResults(allResults);
		return new OverallTeamResultsViewModel
		{
			Config = _config,
			ResultType = title,
			Results = results(overall)
		};
	}
}