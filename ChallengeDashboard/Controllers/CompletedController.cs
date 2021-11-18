using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers;

public class CompletedController : Controller
{
	private readonly IDataService _dataService;

	public CompletedController(IDataService dataService) => _dataService = dataService;

	public async Task<ViewResult> Index() => View(await GetCompleted());

	private async Task<CompletedViewModel> GetCompleted()
	{
		var results = await _dataService.GetAllResults();
		var overall = new OverallResults(results);

		return new CompletedViewModel
		{
			CourseNames = _dataService.CourseNames,
			Links = _dataService.Links,
			RankedResults = overall.Completed()
		};
	}
}