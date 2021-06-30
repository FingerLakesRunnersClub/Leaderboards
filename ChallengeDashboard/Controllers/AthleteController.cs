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

        public async Task<ViewResult> Similar(uint id) => View(await GetSimilarAthletes(id));

        private async Task<AthleteSummaryViewModel> GetAthlete(uint id)
        {
            var athlete = await _dataService.GetAthlete(id);
            var courses = (await _dataService.GetAllResults()).ToList();
            var summary = new AthleteSummary(athlete, courses);

            return new AthleteSummaryViewModel
            {
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                Summary = summary
            };
        }

        private async Task<AthleteCourseResultsViewModel> GetResults(uint id, uint courseID)
        {
            var course = await _dataService.GetResults(courseID);
            var results = course.Results.Where(r => r.Athlete.ID == id).ToList();

            return new AthleteCourseResultsViewModel
            {
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                Athlete = results.First().Athlete,
                Course = course,
                Results = Rank(results, course)
            };
        }

        private static RankedList<Time> Rank(IEnumerable<Result> results, Course course)
        {
            var ranks = new RankedList<Time>();

            var sorted = results.OrderBy(r => r.Duration.Value).ToList();
            foreach(var result in sorted)
            {
                var category = result.Athlete.Category?.Value ?? Category.M.Value;
                var ageGrade = AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(category, result.AgeOnDayOfRun, course.Meters, result.Duration.Value);
                var rank = ageGrade >= 100 ? 0
                    : !ranks.Any() ? 1
                    : ranks.Last().Rank.Value + 1;
                ranks.Add(new Ranked<Time>
                {
                    Rank = ranks.Any() && ranks.Last().Value == result.Duration
                        ? ranks.Last().Rank
                        : new Rank((ushort)rank),
                    Result = result,
                    Value = result.Duration,
                    AgeGrade = new AgeGrade(ageGrade)
                });
            }

            return ranks;
        }

        private async Task<SimilarAthletesViewModel> GetSimilarAthletes(uint id)
        {
            var athlete = await _dataService.GetAthlete(id);
            var results = (await _dataService.GetAllResults()).ToList();
            var my = new AthleteSummary(athlete, results);

            return new SimilarAthletesViewModel
            {
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                Athlete = my.Athlete,
                Matches = Rank(my.SimilarAthletes)
            };
        }
        
        private static IEnumerable<SimilarAthlete> Rank(IEnumerable<SimilarAthlete> matches)
        {
            var ordered = matches.OrderByDescending(r => r.Score).ToArray();
            for (var x = 0; x < ordered.Length; x++)
                ordered[x].Rank = new Rank((ushort)(x + 1));
            return ordered;
        }
    }
}