using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Leaders;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class LeaderboardController(IIterationManager iterationManager, IConfig config) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index(string id = null)
	{
		if (!Enum.TryParse<LeaderboardResultType>(id, out var type))
			type = LeaderboardResultType.Team;
		return View(await GetLeaderboard(type));
	}

	private async Task<ViewModel<Leaderboard>> GetLeaderboard(LeaderboardResultType type)
	{
		var iteration = await iterationManager.ActiveIteration();
		var calculator = new LeaderboardCalculator(config, iteration, type, 3);
		var leaderboard = calculator.GetLeaderboard(type);
		return new ViewModel<Leaderboard>("Leaderboard", leaderboard);
	}
}