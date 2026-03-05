using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area(nameof(Admin))]
[Authorize(nameof(Admin))]
public sealed class HomeController : Controller
{
	[HttpGet]
	public ViewResult Index()
	{
		var vm = new ViewModel<string>("Admin Menu", null);
		return View(vm);
	}
}