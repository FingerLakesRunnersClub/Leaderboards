using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area("Admin")]
public sealed class ResultsController(IImportManager importManager, ICourseService courseService, IRaceService raceService, IResultService resultService) : Controller
{
	public async Task<ViewResult> Index(Guid id)
	{
		var results = await resultService.Find(id);
		var vm = new ViewModel<Result[]>("Course Results", results);
		return View(vm);
	}

	[HttpGet]
	public async Task<ViewResult> Import()
	{
		var races = await raceService.GetAllRaces();
		var form = new ResultImportForm
		{
			Races = races,
			Sources = importManager.Sources
		};
		var vm = new ViewModel<ResultImportForm>("Results Importer", form);
		return View(vm);
	}

	[HttpPost]
	public async Task<RedirectToActionResult> Import(Guid courseID, string source, uint externalID)
	{
		var course = await courseService.Get(courseID);
		await importManager.ImportResults(course, source, externalID);
		return RedirectToAction(nameof(Index), courseID);
	}
}