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

	public async Task<ViewResult> Fastest(uint id, string name, string category = null, byte? ag = null)
	{
		var filter = new Filter { Category = Category.Parse(category), AgeGroup = ag.HasValue ? Athlete.Teams[ag.Value] : null };
		return View(await GetResults(id, name, ResultType.Fastest, filter, c => c.Fastest(filter)));
	}

	public async Task<ViewResult> Farthest(uint id, string name, string category = null, byte? ag = null)
	{
		var filter = new Filter { Category = Category.Parse(category), AgeGroup = ag.HasValue ? Athlete.Teams[ag.Value] : null };
		return View(await GetResults(id, name, ResultType.Farthest, filter, c => c.Farthest(filter)));
	}

	public async Task<ViewResult> BestAverage(uint id, string name, string category = null)
	{
		var filter = new Filter { Category = Category.Parse(category) };
		return View(await GetResults(id, name, ResultType.BestAverage, filter, c => c.BestAverage(filter)));
	}

	public async Task<ViewResult> MostRuns(uint id, string name)
		=> View(await GetResults(id, name, ResultType.MostRuns, Filter.None, c => c.MostRuns(Filter.None)));

	public async Task<ViewResult> Community(uint id, string name)
		=> View(await GetResults(id, name, ResultType.Community, Filter.None, c => c.CommunityStars(Filter.None)));

	private async Task<CourseResultsViewModel<T>> GetResults<T>(uint courseID, string name, ResultType resultType, Filter filter, Func<Course, RankedList<T>> results)
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

	public async Task<ViewResult> Team(uint id, string name)
		=> View(await GetTeamResults(id, name));

	private async Task<CourseResultsViewModel<TeamResults>> GetTeamResults(uint courseID, string name)
	{
		var course = await _dataService.GetResults(courseID, name);
		return new CourseResultsViewModel<TeamResults>
		{
			Config = _config,
			ResultType = new FormattedResultType(ResultType.Team),
			Filter = Filter.None,
			Course = course,
			RankedResults = course.TeamPoints(Filter.None)
		};
	}
}