using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AwardsController(IIterationManager iterationManager, IAwardsCalculator calculator) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index()
	{
		var iteration = await iterationManager.ActiveIteration();
		var awards = calculator.GetAwards(iteration);
		var vm = new ViewModel<Dictionary<Athlete, Award[]>>("Awards", awards);
		return View(vm);
	}
}
