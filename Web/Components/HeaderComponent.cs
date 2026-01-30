using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Data.Models;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FLRC.Leaderboards.Web.Components;

public sealed class HeaderComponent(ISeriesService seriesService, IHttpContextAccessor httpContextAccessor, IConfig config) : ViewComponent
{
	public async Task<ViewViewComponentResult> InvokeAsync()
	{
		var context = AppContext.GetData("Context")!.ToString();
		var series = await seriesService.FindSeries(context);

		var vm = new HeaderViewModel
		{
			Title = config.App,
			ReportMenu = GetReportMenu(series),
			CourseMenuLabel = config.CourseLabel,
			CourseMenu = config.CourseNames,
			LinksMenu = config.Links,
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