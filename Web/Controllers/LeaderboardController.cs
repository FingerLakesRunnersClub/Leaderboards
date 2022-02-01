using FLRC.Leaderboards.Core;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class LeaderboardController : Controller
{
	private readonly IDataService _dataService;

	public LeaderboardController(IDataService dataService) => _dataService = dataService;

	public async Task<ViewResult> Index(string id = null)
	{
		if (!Enum.TryParse<LeaderboardResultType>(id, out var type))
			type = LeaderboardResultType.Team;
		return View(await GetLeaderboard(type));
	}

	private async Task<LeaderboardViewModel> GetLeaderboard(LeaderboardResultType type)
	{
		var results = await _dataService.GetAllResults();
		return new LeaderboardViewModel(results, type)
		{
			Links = _dataService.Links
		};
	}
}