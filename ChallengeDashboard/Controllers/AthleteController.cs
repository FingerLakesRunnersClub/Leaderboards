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

        public async Task<ViewResult> Course(uint id, uint other) => View(await GetResults(id, other));

        private async Task<AthleteCourseViewModel> GetResults(uint id, uint courseID)
        {
            var course = await _dataService.GetResults(courseID);
            var results = course.Results.Where(r => r.Athlete.ID == id).ToList();
            var athlete = results.First().Athlete;

            return new AthleteCourseViewModel
            {
                CourseNames = _dataService.CourseNames,
                Athlete = athlete,
                Course = course,
                Results = results
            };
        }

        private async Task<AthleteSummaryViewModel> GetAthlete(uint id)
        {
            var courses = (await _dataService.GetAllResults()).ToList();
            var overallViewModel = new OverallViewModel(courses);
            var allResults = courses.ToDictionary(c => c, c => c.Results.Where(r => r.Athlete.ID == id));
            var athlete = allResults.First(r => r.Value.Any()).Value.First().Athlete;

            return new AthleteSummaryViewModel
            {
                CourseNames = _dataService.CourseNames,
                Athlete = athlete,
                TeamResults = overallViewModel.TeamPoints().First(r => r.Team == athlete.Team),
                OverallPoints = overallViewModel.MostPoints(athlete.Category).FirstOrDefault(r => r.Athlete.ID == id),
                OverallMiles = overallViewModel.MostMiles(athlete.Category).FirstOrDefault(r => r.Athlete.ID == id),
                Fastest = courses.ToDictionary(c => c, c => c.Fastest().FirstOrDefault(r => r.Athlete.ID == id)),
                Average = courses.ToDictionary(c => c, c => c.BestAverage().FirstOrDefault(r => r.Athlete.ID == id)),
                Runs = courses.ToDictionary(c => c, c => c.MostRuns().FirstOrDefault(r => r.Athlete.ID == id))
            };
        }
    }
}
