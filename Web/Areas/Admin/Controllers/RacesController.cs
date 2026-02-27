using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area("Admin")]
public sealed class RacesController(IRaceService raceService) : Controller
{
	public async Task<ViewResult> Index()
	{
		var races = await raceService.GetAllRaces();
		var vm = new ViewModel<Race[]>("Race Administration", races);
		return View(vm);
	}

	[HttpGet]
	public ViewResult Add()
	{
		var vm = new ViewModel<Race>("Add Race", new Race());
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectToActionResult> Add(Race race, IDictionary<Guid, Course> courses)
	{
		await raceService.AddRace(race, courses);
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public async Task<ViewResult> Edit(Guid id)
	{
		var race = await raceService.GetRace(id);
		var vm = new ViewModel<Race>($"Edit {race.Name}", race);
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectToActionResult> Edit(Guid id, Race updated, IDictionary<Guid, Course> courses)
	{
		var race = await raceService.GetRace(id);
		await raceService.UpdateRace(race, updated, courses);
		return RedirectToAction(nameof(Index));
	}
}