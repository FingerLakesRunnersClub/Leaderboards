using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class AthleteController : Controller
    {
        private readonly IDataService _dataService;
        public AthleteController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Index(uint id) => View(await GetAthlete(id));

        private async Task<AthleteViewModel> GetAthlete(uint id)
        {
            var courses = (await _dataService.GetAllResults()).ToList();
            var results = courses.ToDictionary(c => c, c => c.Results.Where(r => r.Athlete.ID == id));
            var athlete = results.First(r => r.Value.Any()).Value.First().Athlete;
            
            return new AthleteViewModel
            {
                CourseNames = _dataService.CourseNames,
                Athlete = athlete,
                Results = results
            };
        }
    }
}
