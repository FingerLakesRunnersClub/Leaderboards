using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class UltraChallengesController(IIterationManager iterationManager) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Results(Guid? id)
	{
		var iteration = await iterationManager.ActiveIteration();
		var challenges = iteration.UltraChallenges;
		var challenge = id is not null ? challenges.First(c => c.ID == id) : challenges.FirstOrDefault();
		var results = await UltraChallengeResultsCalculator.Earliest(iteration);
		var vm = new ChallengeViewModel
		{
			Challenges = challenges,
			Challenge = challenge,
			Results = results.TryGetValue(challenge, out var result) ?  result : []
		};
		return View(vm);
	}
}