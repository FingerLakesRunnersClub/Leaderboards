using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class CourseController : Controller
{
	private readonly IDataService _dataService;
	private readonly Config _config;

	public CourseController(IDataService dataService, Config config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Fastest(uint id, string other = null, byte? ag = null)
	{
		if (ag != null)
			ViewBag.AgeGroup = ag;
		var category = DataParser.ParseCategory(other);
		return View(await GetResults(id, ResultType.Fastest, category, c => c.Fastest(category, ag)));
	}

	public async Task<ViewResult> MostRuns(uint id, string other = null)
	{
		var category = DataParser.ParseCategory(other);
		return View(await GetResults(id, ResultType.MostRuns, category, c => c.MostRuns(category)));
	}

	public async Task<ViewResult> BestAverage(uint id, string other = null)
	{
		var category = DataParser.ParseCategory(other);
		return View(await GetResults(id, ResultType.BestAverage, category, c => c.BestAverage(category)));
	}

	public async Task<ViewResult> Team(uint id) => View(await GetTeamResults(id));

	private async Task<CourseTeamResultsViewModel> GetTeamResults(uint courseID)
	{
		var course = await _dataService.GetResults(courseID);
		return new CourseTeamResultsViewModel
		{
			Config = _config,
			ResultType = new FormattedResultType(ResultType.Team),
			Course = course,
			Results = course.TeamPoints(),
		};
	}

	private async Task<CourseResultsViewModel<T>> GetResults<T>(uint courseID, ResultType resultType, Category category, Func<Course, RankedList<T>> results)
	{
		var course = await _dataService.GetResults(courseID);
		return new CourseResultsViewModel<T>
		{
			Config = _config,
			ResultType = new FormattedResultType(resultType),
			Category = category,
			Course = course,
			RankedResults = results(course),
		};
	}
}