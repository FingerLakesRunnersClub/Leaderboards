using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FLRC.Leaderboards.Web.Components;

public sealed class HeadComponent(ISeriesService seriesService, IContextProvider contextProvider) : ViewComponent
{
	public async Task<ViewViewComponentResult> InvokeAsync(string title)
	{
		var series = await seriesService.FindSeries(contextProvider.App);
		var vm = new HeadViewModel { Context = series.Key, AppName = series.Name, PageTitle = title };
		return View("../Head", vm);
	}
}