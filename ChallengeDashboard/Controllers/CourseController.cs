using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class CourseController : Controller
    {
        private readonly DataService _dataService;

        public CourseController(DataService dataService) => _dataService = dataService;
        
        public async Task<IActionResult> Index(uint id) => View(await _dataService.GetCourse(id));
    }
}
