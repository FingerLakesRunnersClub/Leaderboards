using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Leaders;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class LeaderboardController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public LeaderboardController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index(string id = null, byte? month = null)
	{
		if (!Enum.TryParse<LeaderboardResultType>(id, out var type))
			type = LeaderboardResultType.Team;
		return View(await GetLeaderboard(type, month));
	}

	private async Task<LeaderboardViewModel> GetLeaderboard(LeaderboardResultType type, byte? month)
	{
		var results = await _dataService.GetAllResults();
		return new LeaderboardViewModel(results, type, month)
		{
			Config = _config
		};
	}
}