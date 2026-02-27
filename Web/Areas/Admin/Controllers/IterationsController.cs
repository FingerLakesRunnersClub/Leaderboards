using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area("Admin")]
public sealed class IterationsController(ISeriesService seriesService, IIterationService iterationService, IRaceService raceService, IRegistrationManager registrationManager) : Controller
{
	public async Task<ViewResult> Index()
	{
		var iterations = await iterationService.GetAllIterations();
		var vm = new ViewModel<IGrouping<Series, Iteration>[]>("Iterations", iterations.GroupBy(i => i.Series).ToArray());
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> Add(Guid id)
	{
		var series = await seriesService.GetSeries(id);
		var races = await raceService.GetAllRaces();
		var form = new IterationForm { Iteration = new Iteration(), Races = races };
		var vm = new ViewModel<IterationForm>($"Add Iteration to {series.Name}", form);
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectResult> Add(Guid id, Iteration iteration, Guid[] races)
	{
		var raceObjects = await raceService.GetRaces(races);

		await iterationService.AddIteration(id, iteration);
		await iterationService.UpdateRaces(iteration, raceObjects);

		return Redirect("/Admin/Iterations");
	}

	[HttpGet]
	public async Task<ViewResult> Edit(Guid id)
	{
		var iteration = await iterationService.GetIteration(id);
		var races = await raceService.GetAllRaces();
		var form = new IterationForm { Iteration = iteration, Races = races };
		var vm = new ViewModel<IterationForm>($"Edit {iteration.Series.Name} {iteration.Name}", form);
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectResult> Edit(Guid id, Iteration updated, Guid[] races)
	{
		var iteration = await iterationService.GetIteration(id);
		var raceObjects = await raceService.GetRaces(races);

		await iterationService.UpdateIteration(iteration, updated);
		await iterationService.UpdateRaces(iteration, raceObjects);

		return Redirect("/Admin/Iterations");
	}

	public async Task<ViewResult> Registration(Guid id)
	{
		var iteration = await iterationService.GetIteration(id);
		var vm = new ViewModel<Athlete[]>($"{iteration.Series.Name} {iteration.Name} Registered Athletes", iteration.Athletes.ToArray());
		return View(vm);
	}

	[HttpPost]
	public async Task<RedirectToActionResult> Registration(Guid id, IFormCollection form)
	{
		var iteration = await iterationService.GetIteration(id);
		await registrationManager.Update(iteration);
		return RedirectToAction(nameof(Registration));
	}
}