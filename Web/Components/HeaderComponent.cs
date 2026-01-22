using FLRC.Leaderboards.Core.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace FLRC.Leaderboards.Web.Components;

public sealed class HeaderComponent : ViewComponent
{
	private readonly IConfig _config;

	public HeaderComponent(IConfig config)
		=> _config = config;

	public ViewViewComponentResult Invoke()
		=> View("../Header", _config);
}