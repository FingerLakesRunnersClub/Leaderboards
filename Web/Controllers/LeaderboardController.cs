using FLRC.Leaderboards.Core.Leaders;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class LeaderboardController(IIterationManager iterationManager, ILeaderboardCalculator calculator) : Controller
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
		var leaderboard = calculator.GetLeaderboard(iteration, type, 3);
		return new ViewModel<Leaderboard>("Leaderboard", leaderboard);
	}
}