using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class CourseController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public CourseController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	[HttpGet]
	public async Task<ViewResult> Fastest(uint id, string name, string category = null, byte? ag = null)
	{
		var filter = new Core.Results.Filter { Category = Category.Parse(category), AgeGroup = ag.HasValue ? Core.Teams.Team.Teams[ag.Value] : null };
		return View(await GetResults(id, name, ResultType.Fastest, filter, c => c.Fastest(filter)));
	}

	[HttpGet]
	public async Task<ViewResult> Farthest(uint id, string name, string category = null, byte? ag = null)
	{
		var filter = new Core.Results.Filter { Category = Category.Parse(category), AgeGroup = ag.HasValue ? Core.Teams.Team.Teams[ag.Value] : null };
		return View(await GetResults(id, name, ResultType.Farthest, filter, c => c.Farthest(filter)));
	}

	[HttpGet]
	public async Task<ViewResult> BestAverage(uint id, string name, string category = null)
	{
		var filter = new Core.Results.Filter { Category = Category.Parse(category) };
		return View(await GetResults(id, name, ResultType.BestAverage, filter, c => c.BestAverage(filter)));
	}

	[HttpGet]
	public async Task<ViewResult> MostRuns(uint id, string name)
		=> View(await GetResults(id, name, ResultType.MostRuns, Core.Results.Filter.None, c => c.MostRuns(Core.Results.Filter.None)));

	[HttpGet]
	public async Task<ViewResult> Community(uint id, string name)
		=> View(await GetResults(id, name, ResultType.Community, Core.Results.Filter.None, c => c.CommunityStars(Core.Results.Filter.None)));

	private async Task<CourseResultsViewModel<T>> GetResults<T>(uint courseID, string name, ResultType resultType, Core.Results.Filter filter, Func<Course, RankedList<T, Result>> results)
	{
		var course = await _dataService.GetResults(courseID, name);
		return new CourseResultsViewModel<T>
		{
			Config = _config,
			ResultType = new FormattedResultType(resultType),
			Filter = filter,
			Course = course,
			RankedResults = results(course)
		};
	}

	[HttpGet]
	public async Task<ViewResult> Team(uint id, string name)
		=> View(await GetTeamResults(id, name));

	private async Task<CourseResultsViewModel<TeamResults>> GetTeamResults(uint courseID, string name)
	{
		var course = await _dataService.GetResults(courseID, name);
		return new CourseResultsViewModel<TeamResults>
		{
			Config = _config,
			ResultType = new FormattedResultType(ResultType.Team),
			Filter = Core.Results.Filter.None,
			Course = course,
			RankedResults = course.TeamPoints(Core.Results.Filter.None)
		};
	}
}