using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FLRC.Leaderboards.Web.Components;

public sealed class Header(IAuthService authService, IAdminService adminService, IAthleteService athleteService, IConfig config, IIterationManager iterationManager) : ViewComponent
{
	public async Task<ViewViewComponentResult> InvokeAsync()
	{
		var iteration = await iterationManager.ActiveIteration();
		var officialCourses = iteration.OfficialChallenge?.Courses.OrderBy(c => new Distance(c.DistanceDisplay).Meters).ToArray();
		var otherCourses = iteration.Races.SelectMany(r => r.Courses).Except(iteration.OfficialChallenge?.Courses ?? []).OrderBy(c => c.Race.Name).ToArray();

		var enableAuth = iteration.Series.Features.FirstOrDefault(f => f.Key == nameof(FeatureSet.EnableAuth))?.Value ?? false;

		var user = enableAuth && authService.IsLoggedIn()
			? authService.GetCurrentUser()
			: null;

		var athlete = user is not null
			? await athleteService.Find(LinkedAccount.Keys.Discourse, user.ClaimDictionary["external_id"])
			: null;

		var admin = athlete is not null && await adminService.Verify(athlete.ID);

		var vm = new HeaderViewModel
		{
			Series = iteration.Series,
			ReportMenu = GetReportMenu(iteration.Series),
			CourseMenuLabel = iteration.Series.Settings.FirstOrDefault(s => s.Key == nameof(IConfig.CourseLabel))?.Value,
			OfficialCourses = officialCourses,
			OtherCourses = otherCourses,
			LinksMenu = config.Links,
			EnableAuth = iteration.Series.Features.FirstOrDefault(f => f.Key == nameof(FeatureSet.EnableAuth))?.Value ?? false,
			User = user,
			Athlete = athlete,
			IsAdmin = admin
		};

		return View("../Header", vm);
	}

	private Dictionary<string, string> GetReportMenu(Series series)
	{
		var reports = new Dictionary<string, string>();
		var features = series.Features.Where(f => f.Value).Select(f => f.Key).ToArray();

		if (features.Contains(nameof(FeatureSet.MultiAttempt)))
			reports.Add("Activity Log", "/Log");

		if (config.Awards.Any())
			reports.Add("Awards", "/Awards");

		if (features.Contains(nameof(FeatureSet.MultiAttemptCompetitions)))
			reports.Add("Completions", "/Completed");

		reports.Add("Participants", "/Athletes");
		reports.Add("Statistics", "/Statistics");

		if (config.SeriesTitle is not null)
			reports.Add(config.SeriesTitle, "/UltraChallenges/Results");

		return reports;
	}
}