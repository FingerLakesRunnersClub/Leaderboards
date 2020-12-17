using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDataService _dataService;

        public DashboardController(IDataService dataService) => _dataService = dataService;

        public async Task<IActionResult> Index()
        {
            ViewBag.CourseNames = _dataService.CourseNames;
            return View(await _dataService.GetAllResults());
        }
    }
}
