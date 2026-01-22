using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class HomeController : Controller
{
	public ViewResult Index()
	{
		var vm = new ViewModel<string>("Admin Menu", null);
		return View(vm);
	}
}