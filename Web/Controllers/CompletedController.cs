using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class CompletedController(IIterationManager iterationManager, ICommunityStarCalculator starCalculator) : Controller
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
		var overall = new OverallResultsCalculator(starCalculator, iteration);

		return new Completed
		{
			Results = overall.Completed(),
			PersonalResults = overall.CompletedPersonal()
		};
	}
}