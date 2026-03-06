using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area(nameof(Admin))]
[Authorize(nameof(Admin))]
public sealed class SeriesController(ISeriesService seriesService) : Controller
{
	public async Task<ViewResult> Index()
	{
		var series = await seriesService.All();
		var vm = new ViewModel<Series[]>("Series Management", series);
		return View(vm);
	}

	[HttpGet]
	public ViewResult Add()
	{
		var vm = new ViewModel<Series>("Add Series", new Series());
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectToActionResult> Add(Series series, IDictionary<string, bool> features, IDictionary<string, string> settings)
	{
		await seriesService.Add(series);
		await seriesService.UpdateFeatures(series, features);
		await seriesService.UpdateSettings(series, settings);

		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public async Task<ViewResult> Edit(Guid id)
	{
		var series = await seriesService.Get(id);
		var vm = new ViewModel<Series>("Edit Series", series);
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectToActionResult> Edit(Guid id, Series updated, IDictionary<string, bool> features, IDictionary<string, string> settings)
	{
		var series = await seriesService.Get(id);

		await seriesService.Update(series, updated);
		await seriesService.UpdateFeatures(series, features);
		await seriesService.UpdateSettings(series, settings);

		return RedirectToAction(nameof(Index));
	}
}