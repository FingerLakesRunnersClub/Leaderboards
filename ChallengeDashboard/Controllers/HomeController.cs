using Microsoft.AspNetCore.Mvc;

namespace ChallengeDashboard.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
