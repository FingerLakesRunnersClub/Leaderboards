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

public sealed class OverallController(IIterationManager iterationManager, IOverallResultsCalculator overall, IConfig config) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Points(string id)
	{
		var filter = new Filter(Category.Parse(id));
		return View(await GetResults(config.Competitions[$"Points/{id}"], (o, i) =>  o.MostPoints(i, filter)));
	}

	[HttpGet]
	public async Task<ViewResult> PointsTop3(string id)
	{
		var filter = new Filter(Category.Parse(id));
		return View("Points", await GetResults(config.Competitions[$"PointsTop3/{id}"], (o, i) =>  o.MostPoints(i, 3, filter)));
	}

	[HttpGet]
	public async Task<ViewResult> Miles()
		=> View(await GetResults(config.Competitions["Miles"], (o, i) =>  o.MostMiles(i, new Filter())));

	[HttpGet]
	public async Task<ViewResult> Courses()
		=> View(await GetResults(config.Competitions["Courses"], (o, i) =>  o.MostCourses(i, new Filter())));

	[HttpGet]
	public async Task<ViewResult> AgeGrade()
		=> View(await GetResults(config.Competitions["AgeGrade"], (o, i) =>  o.AgeGrade(i, new Filter())));

	[HttpGet]
	public async Task<ViewResult> Community()
		=> View(await GetResults(config.Competitions["Community"], (o, i) =>  o.Community(i, new Filter())));

	private async Task<ViewModel<OverallResults<T>>> GetResults<T>(string title, Func<IOverallResultsCalculator, Iteration, RankedList<T, Result>> results)
	{
		var iteration = await iterationManager.ActiveIteration();
		var rankedResults = results(overall, iteration);

		var data = new OverallResults<T>
		{
			Config = config,
			ResultType = title,
			RankedResults = rankedResults
		};
		return new ViewModel<OverallResults<T>>("Overall Results", data);
	}

	public async Task<ViewResult> Team()
		=> View(await GetTeamResults(config.Competitions["Team"], (o, i) =>  o.TeamPoints(i)));

	private async Task<ViewModel<OverallResults<TeamResults>>> GetTeamResults(string title, Func<IOverallResultsCalculator, Iteration, RankedList<TeamResults, Result>> results)
	{
		var iteration = await iterationManager.ActiveIteration();
		var data = new OverallResults<TeamResults>
		{
			Config = config,
			ResultType = title,
			RankedResults = results(overall, iteration)
		};
		return new ViewModel<OverallResults<TeamResults>>("Team", data);
	}
}