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

public sealed class OverallController(IIterationManager iterationManager, ICommunityStarCalculator starCalculator, IConfig config) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Points(string id)
	{
		var filter = new Filter(Category.Parse(id));
		return View(await GetResults(config.Competitions[$"Points/{id}"], overall => overall.MostPoints(filter)));
	}

	[HttpGet]
	public async Task<ViewResult> PointsTop3(string id)
	{
		var filter = new Filter(Category.Parse(id));
		return View("Points", await GetResults(config.Competitions[$"PointsTop3/{id}"], overall => overall.MostPoints(3, filter)));
	}

	[HttpGet]
	public async Task<ViewResult> Miles()
		=> View(await GetResults(config.Competitions["Miles"], overall => overall.MostMiles(new Filter())));

	[HttpGet]
	public async Task<ViewResult> Courses()
		=> View(await GetResults(config.Competitions["Courses"], overall => overall.MostCourses(new Filter())));

	[HttpGet]
	public async Task<ViewResult> AgeGrade()
		=> View(await GetResults(config.Competitions["AgeGrade"], overall => overall.AgeGrade(new Filter())));

	[HttpGet]
	public async Task<ViewResult> Community()
		=> View(await GetResults(config.Competitions["Community"], overall => overall.Community(new Filter())));

	private async Task<ViewModel<OverallResults<T>>> GetResults<T>(string title, Func<OverallResultsCalculator, RankedList<T, Result>> results)
	{
		var iteration = await iterationManager.ActiveIteration();
		var calculator = new OverallResultsCalculator(starCalculator, iteration);
		var rankedResults = results(calculator);

		var overall = new OverallResults<T>
		{
			Config = config,
			ResultType = title,
			RankedResults = rankedResults
		};
		return new ViewModel<OverallResults<T>>("Overall Results", overall);
	}

	public async Task<ViewResult> Team()
		=> View(await GetTeamResults(config.Competitions["Team"], overall => overall.TeamPoints()));

	private async Task<ViewModel<OverallResults<TeamResults>>> GetTeamResults(string title, Func<OverallResultsCalculator, RankedList<TeamResults, Result>> results)
	{
		var iteration = await iterationManager.ActiveIteration();
		var overall = new OverallResultsCalculator(starCalculator, iteration);
		var data = new OverallResults<TeamResults>
		{
			Config = config,
			ResultType = title,
			RankedResults = results(overall)
		};
		return new ViewModel<OverallResults<TeamResults>>("Team", data);
	}
}