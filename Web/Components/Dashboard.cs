using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Components;

public sealed class Dashboard(IAuthService authService, IAthleteService athleteService, IIterationManager iterationManager)
	: ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync(bool showLinkButton)
	{
		if (!authService.IsLoggedIn())
			return Content(string.Empty);

		var user = authService.GetCurrentUser();
		var athlete = await athleteService.Find(LinkedAccount.Keys.Discourse, user.ClaimDictionary["external_id"]);

		if (athlete is null)
			return Content(string.Empty);

		var iteration = await iterationManager.ActiveIteration();
		if (iteration is null || iteration.EndDate < DateOnly.FromDateTime(DateTime.Today))
			return Content(string.Empty);

		if (iteration.RegistrationType == nameof(WebScorer) && !athlete.IsRegistered(iteration))
			return View("Register", iteration);

		var challenge = athlete.Challenges.FirstOrDefault(c => c.Iteration == iteration && c.IsPrimary);
		if (challenge is null)
			return View("Select");

		var progress = AthleteProgress.GetProgress(athlete, iteration, showLinkButton);
		return View("Progress", progress);
	}
}