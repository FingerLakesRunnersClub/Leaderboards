using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class IterationsController(ISeriesService seriesService, IIterationService iterationService) : Controller
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
		var vm = new ViewModel<Iteration>($"Add Iteration to {series.Name}", new Iteration());
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectResult> Add(Guid id, Iteration iteration)
	{
		await iterationService.AddIteration(id, iteration);
		return Redirect("/Admin/Iterations");
	}

	[HttpGet]
	public async Task<ViewResult> Edit(Guid id)
	{
		var iteration = await iterationService.GetIteration(id);
		var vm = new ViewModel<Iteration>($"Edit {iteration.Series.Name} {iteration.Name}", iteration);
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectResult> Edit(Guid id, Iteration updated)
	{
		var iteration = await iterationService.GetIteration(id);
		await iterationService.UpdateIteration(iteration, updated);
		return Redirect("/Admin/Iterations");
	}
}