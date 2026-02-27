using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area("Admin")]
public sealed class SeriesController(ISeriesService seriesService) : Controller
{
	public async Task<ViewResult> Index()
	{
		var series = await seriesService.GetAllSeries();
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
	public async Task<RedirectResult> Add(Series series, IDictionary<string, bool> features, IDictionary<string, string> settings)
	{
		await seriesService.AddSeries(series, features, settings);
		return Redirect("/Admin/Series");
	}

	[HttpGet]
	public async Task<ViewResult> Edit(Guid id)
	{
		var series = await seriesService.GetSeries(id);
		var vm = new ViewModel<Series>("Edit Series", series);
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectResult> Edit(Guid id, Series updated, IDictionary<string, bool> features, IDictionary<string, string> settings)
	{
		var series = await seriesService.GetSeries(id);
		await seriesService.UpdateSeries(series, updated, features, settings);
		return Redirect("/Admin/Series");
	}
}