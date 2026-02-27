using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FLRC.Leaderboards.Web.Components;

public sealed class HeaderComponent(ISeriesService seriesService, IHttpContextAccessor httpContextAccessor, IConfig config, IContextProvider contextProvider) : ViewComponent
{
	public async Task<ViewViewComponentResult> InvokeAsync()
	{
		var series = await seriesService.FindSeries(contextProvider.App);
		var enableAuth = series.Features.FirstOrDefault(f => f.Key == nameof(FeatureSet.EnableAuth))?.Value ?? false;

		var vm = new HeaderViewModel
		{
			Series = series,
			ReportMenu = GetReportMenu(series),
			CourseMenuLabel = config.CourseLabel,
			CourseMenu = config.CourseNames,
			LinksMenu = config.Links,
			EnableAuth = enableAuth,
			User = httpContextAccessor.HttpContext!.User
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
			reports.Add(config.SeriesTitle, "/Series");

		return reports;
	}
}