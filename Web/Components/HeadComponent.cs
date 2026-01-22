using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FLRC.Leaderboards.Web.Components;

public sealed class HeadComponent : ViewComponent
{
	private readonly string _appName;

	public HeadComponent(IConfig config)
		=> _appName = config.App;

	public ViewViewComponentResult Invoke(string title)
	{
		var vm = new HeadViewModel { AppName = _appName, PageTitle = title };
		return View("../Head", vm);
	}
}