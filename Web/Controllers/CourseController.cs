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

	public async Task<ViewResult> Fastest(uint id, string distance, string category = null, byte? month = null, byte? ag = null)
	{
		var filter = new Filter { Category = Category.Parse(category), Month = month, AgeGroup = ag.HasValue ? Athlete.Teams[ag.Value] : null };
		return View(await GetResults(id, distance, ResultType.Fastest, filter, c => c.Fastest(filter)));
	}

	public async Task<ViewResult> BestAverage(uint id, string distance, string category = null, byte? month = null)
	{
		var filter = new Filter { Category = Category.Parse(category), Month = month };
		return View(await GetResults(id, distance, ResultType.BestAverage, filter, c => c.BestAverage(filter)));
	}

	public async Task<ViewResult> MostRuns(uint id, string distance, byte? month = null)
	{
		var filter = new Filter { Month = month };
		return View(await GetResults(id, distance, ResultType.MostRuns, filter, c => c.MostRuns(filter)));
	}

	public async Task<ViewResult> Community(uint id, string distance, byte? month = null)
	{
		var filter = new Filter { Month = month };
		return View(await GetResults(id, distance, ResultType.Community, filter, c => c.CommunityStars(filter)));
	}

	private async Task<CourseResultsViewModel<T>> GetResults<T>(uint courseID, string distance, ResultType resultType, Filter filter, Func<Course, RankedList<T>> results)
	{
		var course = await _dataService.GetResults(courseID, distance);
		return new CourseResultsViewModel<T>
		{
			Config = _config,
			ResultType = new FormattedResultType(resultType),
			Filter = filter,
			Course = course,
			Months = course.DistinctMonths(),
			RankedResults = results(course),
		};
	}

	public async Task<ViewResult> Team(uint id, string distance, byte? month = null) => View(await GetTeamResults(id, distance, month));

	private async Task<CourseResultsViewModel<TeamResults>> GetTeamResults(uint courseID, string distance, byte? month = null)
	{
		var course = await _dataService.GetResults(courseID, distance);
		var filter = new Filter { Month = month };
		return new CourseResultsViewModel<TeamResults>
		{
			Config = _config,
			ResultType = new FormattedResultType(ResultType.Team),
			Filter = filter,
			Course = course,
			Months = course.DistinctMonths(),
			RankedResults = course.TeamPoints(filter),
		};
	}
}