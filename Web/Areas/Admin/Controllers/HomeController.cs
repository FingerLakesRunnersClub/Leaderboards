using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

public sealed class HomeController : AdminController
{
	public ViewResult Index()
	{
		var vm = new ViewModel<string>("Admin Menu", null);
		return View(vm);
	}
}