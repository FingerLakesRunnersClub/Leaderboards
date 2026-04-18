using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FLRC.Leaderboards.Web.Components;

public class AddResultButton(IIterationManager iterationManager, IAuthService authService, IAthleteService athleteService) : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync(Course course)
	{
		var loggedIn = authService.IsLoggedIn();
		if (!loggedIn)
			return Empty;

		var iteration = await iterationManager.ActiveIteration();
		var iterationIsActive = iteration?.IsActive ?? false;
		if (!iterationIsActive)
			return Empty;

		var hasSelfTiming = iteration.Series.Feature[nameof(FeatureSet.SelfTiming)];
		if (!hasSelfTiming)
			return Empty;

		var courseIsInActiveIteration = course.Race.Iterations.Contains(iteration);
		if (!courseIsInActiveIteration)
			return Empty;

		var athlete = await CurrentAthlete();
		var athleteIsRegistered = athlete?.Registrations.Contains(iteration) ?? false;
		if (!athleteIsRegistered)
			return Empty;

		return View("../../../Results/AddResultButton", course.ID);
	}

	private ContentViewComponentResult Empty => Content(string.Empty);

	private async Task<Athlete> CurrentAthlete()
	{
		var user = authService.GetCurrentUser();
		var claims = user.ClaimDictionary;

		return await athleteService.Find(LinkedAccount.Keys.Discourse, claims["external_id"]);
	}
}