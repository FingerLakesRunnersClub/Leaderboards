using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class CourseController : Controller
    {
        private readonly IDataService _dataService;

        public CourseController(IDataService dataService) => _dataService = dataService;
        
        public async Task<IActionResult> Index(uint id) => View(await _dataService.GetCourse(id));
    }
}
