using System.Collections.Generic;
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

        private async Task<AthleteCourseResultsViewModel> GetResults(uint id, uint courseID)
        {
            var course = await _dataService.GetResults(courseID);
            var results = course.Results.Where(r => r.Athlete.ID == id).ToList();
            var athlete = results.First().Athlete;

            return new AthleteCourseResultsViewModel
            {
                CourseNames = _dataService.CourseNames,
                Athlete = athlete,
                Course = course,
                Results = Rank(results, course)
            };
        }

        private RankedList<Time> Rank(IEnumerable<Result> results, Course course)
        {
            var ranks = new RankedList<Time>();

            var sorted = results.OrderBy(r => r.Duration.Value).ToList();
            for (byte rank = 1; rank <= sorted.Count; rank++)
            {
                var result = sorted[rank - 1];
                ranks.Add(new Ranked<Time>
                {
                    Rank = ranks.Any() && ranks.Last().Value.Equals(result.Duration)
                        ? ranks.Last().Rank
                        : new Rank(rank),
                    Result = result,
                    Value = result.Duration,
                    AgeGrade = new AgeGrade(result.Athlete.Category != null
                        ? AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(result.Athlete.Category.Value ?? throw null,
                            result.Athlete.Age, course.Meters, result.Duration.Value)
                        : 0)
                });
            }

            return ranks;
        }

        private async Task<AthleteSummaryViewModel> GetAthlete(uint id)
        {
            var athlete = await _dataService.GetAthlete(id);
            var courses = (await _dataService.GetAllResults()).ToList();
            var overallViewModel = new OverallViewModel(courses);

            return new AthleteSummaryViewModel
            {
                CourseNames = _dataService.CourseNames,
                Athlete = athlete,
                TeamResults = overallViewModel.TeamPoints().First(r => r.Team == athlete.Team),
                OverallPoints = overallViewModel.MostPoints(athlete.Category).FirstOrDefault(r => r.Result.Athlete.ID == id),
                OverallMiles = overallViewModel.MostMiles(athlete.Category).FirstOrDefault(r => r.Result.Athlete.ID == id),
                Fastest = courses.ToDictionary(c => c, c => c.Fastest().FirstOrDefault(r => r.Result.Athlete.ID == id)),
                Average = courses.ToDictionary(c => c, c => c.BestAverage().FirstOrDefault(r => r.Result.Athlete.ID == id)),
                Runs = courses.ToDictionary(c => c, c => c.MostRuns().FirstOrDefault(r => r.Result.Athlete.ID == id))
            };
        }
    }
}