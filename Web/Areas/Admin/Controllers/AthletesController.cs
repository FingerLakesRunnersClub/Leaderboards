using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area(nameof(Admin))]
[Authorize(nameof(Admin))]
public sealed class AthletesController(IAthleteService athleteService) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index()
	{
		var athletes = await athleteService.All();
		var vm = new ViewModel<Athlete[]>("Athlete Manager", athletes);
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> Edit(Guid id)
	{
		var athlete = await athleteService.Get(id);
		var vm = new ViewModel<Athlete>("Edit Athlete", athlete);
		return View("Form", vm);
	}

	[HttpPost]
	public async Task<RedirectToActionResult> Edit(Guid id, Athlete updated)
	{
		var athlete = await athleteService.Get(id);
		await athleteService.Update(athlete, updated);
		return RedirectToAction(nameof(Edit));
	}

	[HttpGet]
	public async Task<RedirectToActionResult> ToggleAdmin(Guid id)
	{
		var athlete = await athleteService.Get(id);

		if (athlete.IsAdmin)
			await athleteService.RemoveAdmin(athlete);
		else
			await athleteService.AddAdmin(athlete);

		return RedirectToAction(nameof(Edit), new { id });
	}

	[HttpGet]
	public async Task<ViewResult> Merge(Guid id)
	{
		var form = new MergeAthletesForm
		{
			Current = await athleteService.Get(id),
			Athletes = await athleteService.All()
		};
		var vm = new ViewModel<MergeAthletesForm>("Merge Athletes", form);
		return View(vm);
	}

	[HttpPost]
	public async Task<RedirectToActionResult> Merge(Guid id, Guid from, Guid to)
	{
		if (from != id && to != id)
			throw new ArgumentException("As a precaution, the source athlete must be one of the selected values");

		if (from == to)
			throw new ArgumentException("Two different athletes must be selected");

		var old = await athleteService.Get(from);
		var athlete = await athleteService.Get(to);

		await athleteService.MigrateResults(old, athlete);
		await athleteService.MigrateRegistrations(old, athlete);
		await athleteService.MigrateLinkedAccounts(old, athlete);
		await athleteService.RemoveAdmin(old);
		await athleteService.Delete(old);

		return RedirectToAction(nameof(Edit), new { id = to });
	}
}