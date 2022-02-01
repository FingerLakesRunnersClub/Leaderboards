using FLRC.Leaderboards.Core;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Challenge.Controllers;

public class OverallController : Controller
{
	private readonly IDataService _dataService;

	public OverallController(IDataService dataService) => _dataService = dataService;

	public async Task<ViewResult> Points(string id)
	{
		var category = DataParser.ParseCategory(id);
		return View(await GetResults($"Most Points ({id})", vm => vm.MostPoints(category)));
	}

	public async Task<ViewResult> Miles()
		=> View(await GetResults("Most Miles", vm => vm.MostMiles()));

	public async Task<ViewResult> AgeGrade()
		=> View(await GetResults("Age Grade", vm => vm.AgeGrade()));

	public async Task<ViewResult> Team()
		=> View(await GetTeamResults("Team Points", vm => vm.TeamPoints()));

	private async Task<OverallResultsViewModel<T>> GetResults<T>(string title, Func<OverallResults, RankedList<T>> results)
	{
		var allResults = await _dataService.GetAllResults();
		var overall = new OverallResults(allResults);
		return new OverallResultsViewModel<T>
		{
			CourseNames = _dataService.CourseNames,
			Links = _dataService.Links,
			ResultType = title,
			RankedResults = results(overall)
		};
	}

	private async Task<OverallTeamResultsViewModel> GetTeamResults(string title, Func<OverallResults, IEnumerable<TeamResults>> results)
	{
		var allResults = await _dataService.GetAllResults();
		var overall = new OverallResults(allResults);
		return new OverallTeamResultsViewModel
		{
			CourseNames = _dataService.CourseNames,
			Links = _dataService.Links,
			ResultType = title,
			Results = results(overall)
		};
	}
}