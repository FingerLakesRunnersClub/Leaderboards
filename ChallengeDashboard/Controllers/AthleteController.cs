using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class AthleteController : Controller
    {
        private readonly IDataService _dataService;
        public AthleteController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Index(uint id)
        {
            ViewBag.CourseNames = _dataService.CourseNames;
            return View(await GetAthlete(id));
        }

        private async Task<AthleteViewModel> GetAthlete(uint id)
        {
            var courses = (await _dataService.GetAllResults()).ToList();
            var athlete = courses.SelectMany(c => c.Results.Select(r => r.Athlete)).FirstOrDefault(a => a.ID == id);
            var results = courses.Where(c => c.Results.Any(r => r.Athlete == athlete))
                .ToDictionary(c => c, c => c.Results.Where(r => r.Athlete == athlete));
            
            return new AthleteViewModel
            {
                Athlete = athlete,
                Results = results
            };
        }
    }
}
