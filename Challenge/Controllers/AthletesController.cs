using FLRC.Leaderboards.Core;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Challenge.Controllers;

public class AthletesController : Controller
{
	private readonly IDataService _dataService;
	public AthletesController(IDataService dataService) => _dataService = dataService;

	public async Task<ViewResult> Index() => View(await GetAthletes());

	private async Task<AthletesViewModel> GetAthletes()
	{
		return new()
		{
			CourseNames = _dataService.CourseNames,
			Links = _dataService.Links,
			Athletes = await _dataService.GetAthletes()
		};
	}
}