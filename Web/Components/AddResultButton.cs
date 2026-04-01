using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Components;

public class AddResultButton(IAuthService authService, ISeriesService seriesService, IContextProvider contextProvider) : ViewComponent
{
	private static DateOnly Today => DateOnly.FromDateTime(DateTime.Now);

	public async Task<IViewComponentResult> InvokeAsync(Course course)
	{
		var loggedIn = authService.IsLoggedIn();
		var series = await seriesService.Find(contextProvider.App);
		var hasActiveIteration = course.Race.Iterations.Any(i => Today >= i.StartDate && Today <= i.EndDate);

		return loggedIn && series.Feature[nameof(FeatureSet.SelfTiming)] && hasActiveIteration
			? View("../../../Results/AddResultButton", course.ID)
			: Content(string.Empty);
	}
}