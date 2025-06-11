using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Leaders;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class LeaderboardController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public LeaderboardController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index(string id = null)
	{
		if (!Enum.TryParse<LeaderboardResultType>(id, out var type))
			type = LeaderboardResultType.Team;
		return View(await GetLeaderboard(type));
	}

	private async Task<LeaderboardViewModel> GetLeaderboard(LeaderboardResultType type)
	{
		var results = await _dataService.GetAllResults();
		var tableSize = _config.FileSystemResults is not null ? 10 : 3;
		return new LeaderboardViewModel(results, type, (byte)tableSize)
		{
			Config = _config
		};
	}
}