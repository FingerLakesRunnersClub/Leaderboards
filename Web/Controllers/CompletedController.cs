using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class CompletedController(IIterationManager iterationManager, IDataService dataService) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index()
	{
		var completed = await GetCompleted();
		var vm = new ViewModel<Completed>("Completions", completed);
		return View(vm);
	}

	private async Task<Completed> GetCompleted()
	{
		var iteration = await iterationManager.ActiveIteration();
		var overall = new OverallResultsCalculator(iteration);

		return new Completed
		{
			Results = overall.Completed(),
			PersonalResults = await dataService.GetPersonalCompletions()
		};
	}
}