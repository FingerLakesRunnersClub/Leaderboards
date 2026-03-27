using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Result = FLRC.Leaderboards.Model.Result;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class ResultsController(IIterationManager iterationManager, ICourseService courseService) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Fastest(Guid id, Filter filter)
	{
		var iteration = await iterationManager.ActiveIteration();
		var results = await GetResults(id, filter, ResultType.Fastest, (c, f) => c.Results.Fastest(f));
		var vm = new ViewModel<CourseResults<Time>>($"{iteration?.Series.Setting[nameof(AppConfig.CourseLabel)]} Results", results);
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> BestAverage(Guid id, Filter filter)
	{
		var iteration = await iterationManager.ActiveIteration();
		var results = await GetResults(id, filter, ResultType.BestAverage, (c, f) => c.Results.BestAverage(f));
		var vm = new ViewModel<CourseResults<Time>>($"{iteration?.Series.Setting[nameof(AppConfig.CourseLabel)]} Results", results);
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> MostRuns(Guid id, Filter filter)
	{
		var iteration = await iterationManager.ActiveIteration();
		var results = await GetResults(id, filter, ResultType.MostRuns, (c, f) => c.Results.MostRuns(f));
		var vm = new ViewModel<CourseResults<ushort>>($"{iteration?.Series.Setting[nameof(AppConfig.CourseLabel)]} Results", results);
		return View(vm);
	}

	private async Task<CourseResults<T>> GetResults<T>(Guid id, Filter filter, ResultType type, Func<Course, Filter, RankedList<T, Result>> resultsFunc)
	{
		var course = await courseService.Get(id);
		return new CourseResults<T>
		{
			Course = course,
			Type = type,
			Filter = filter,
			Results = resultsFunc(course, filter)
		};
	}
}