using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AthletesController(IIterationManager iterationManager) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null)
			return Redirect("/");

		var athletes = iteration.Athletes.ToArray();
		var vm = new ViewModel<Athlete[]>("Registered Participants", athletes);
		return View(vm);
	}
}