using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class OverallController(IIterationManager iterationManager, IConfig config) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Points(string id)
	{
		var filter = new Filter { Category = Category.Parse(id) };
		var vm = await GetResults(config.Competitions[$"Points/{id}"], overall => overall.MostPoints(filter));
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> PointsTop3(string id)
	{
		var filter = new Filter { Category = Category.Parse(id) };
		var vm = await GetResults(config.Competitions[$"PointsTop3/{id}"], overall => overall.MostPoints(3, filter));
		return View("Points", vm);
	}

	[HttpGet]
	public async Task<ViewResult> Miles()
	{
		var vm = await GetResults(config.Competitions["Miles"], overall => overall.MostMiles());
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> AgeGrade()
	{
		var vm = await GetResults(config.Competitions["AgeGrade"], overall => overall.AgeGrade());
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> Team()
	{
		var vm = await GetTeamResults(config.Competitions["Team"], overall => overall.TeamPoints());
		return View(vm);
	}

	private async Task<ViewModel<OverallResults<T>>> GetResults<T>(string title, Func<OverallResultsCalculator, RankedList<T, Result>> results)
	{
		var iteration = await iterationManager.ActiveIteration();
		var overall = new OverallResultsCalculator(iteration);
		var rankedResults = results(overall);

		var data = new OverallResults<T>
		{
			ResultType = title,
			Results = rankedResults
		};
		return new ViewModel<OverallResults<T>>(title, data);
	}

	private async Task<ViewModel<OverallResults<TeamResults>>> GetTeamResults(string title, Func<OverallResultsCalculator, RankedList<TeamResults, Result>> results)
	{
		var iteration = await iterationManager.ActiveIteration();
		var overall = new OverallResultsCalculator(iteration);
		var data = new OverallResults<TeamResults>
		{
			ResultType = title,
			Results = results(overall)
		};
		return new ViewModel<OverallResults<TeamResults>>(title, data);
	}
}