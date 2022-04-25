using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
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
	private readonly IConfig _config;

	public CourseController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Fastest(uint id, string distance, string category = null, byte? ag = null)
	{
		if (ag != null)
			ViewBag.AgeGroup = ag;
		var cat = Category.Parse(category);
		return View(await GetResults(id, distance, ResultType.Fastest, cat, c => c.Fastest(new Filter(cat, ag.HasValue ? Athlete.Teams[ag.Value] : null))));
	}

	public async Task<ViewResult> MostRuns(uint id, string distance, string category = null)
	{
		var cat = Category.Parse(category);
		return View(await GetResults(id, distance, ResultType.MostRuns, cat, c => c.MostRuns(new Filter(cat))));
	}

	public async Task<ViewResult> BestAverage(uint id, string distance, string category = null)
	{
		var cat = Category.Parse(category);
		return View(await GetResults(id, distance, ResultType.BestAverage, cat, c => c.BestAverage(new Filter(cat))));
	}

	private async Task<CourseResultsViewModel<T>> GetResults<T>(uint courseID, string distance, ResultType resultType, Category category, Func<Course, RankedList<T>> results)
	{
		var course = await _dataService.GetResults(courseID, distance);
		return new CourseResultsViewModel<T>
		{
			Config = _config,
			ResultType = new FormattedResultType(resultType),
			Category = category,
			Course = course,
			RankedResults = results(course),
		};
	}

	public async Task<ViewResult> Team(uint id, string distance) => View(await GetTeamResults(id, distance));

	private async Task<CourseResultsViewModel<TeamResults>> GetTeamResults(uint courseID, string distance)
	{
		var course = await _dataService.GetResults(courseID, distance);
		return new CourseResultsViewModel<TeamResults>
		{
			Config = _config,
			ResultType = new FormattedResultType(ResultType.Team),
			Course = course,
			RankedResults = course.TeamPoints(),
		};
	}

	public async Task<ViewResult> Community(uint id, string distance)
		=> View(await GetResults(id, distance, ResultType.Community, null, c => c.CommunityStars()));
}