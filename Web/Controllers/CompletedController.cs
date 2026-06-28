using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class CompletedController(IIterationManager iterationManager, IOverallResultsCalculator overall) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null)
			return Redirect("/");

		var completed = new Completed
		{
			Iteration = iteration,
			Results = overall.Completed(iteration),
			PersonalResults = overall.CompletedPersonal(iteration)
		};

		var vm = new ViewModel<Completed>("Completed Challenges", completed);
		return View(vm);
	}

	[HttpGet]
	public async Task<IActionResult> Progress()
	{
		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null)
			return Redirect("/");

		var data = iteration.Athletes
			.Select(a => AthleteProgress.GetProgress(a, iteration, false))
			.Where(p => p is not null && p.CoursePercent < 100)
			.OrderByDescending(p => p.CoursePercent)
			.ThenBy(p => p.AllCourses.Length - p.CompletedCourses.Length)
			.ToArray();

		var vm = new ViewModel<ChallengeProgress[]>("In-Progress Challenges", data);
		return View(vm);
	}
}