using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FLRC.Leaderboards.Web.Components;

public sealed class Head(IContextManager contextManager) : ViewComponent
{
	public async Task<ViewViewComponentResult> InvokeAsync(string title)
	{
		var series = await contextManager.Series();
		var vm = new HeadViewModel { Context = series.Key, AppName = series.Name, PageTitle = title };
		return View("../Head", vm);
	}
}