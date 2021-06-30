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

        private async Task<SimilarAthletesViewModel> GetSimilarAthletes(uint id)
        {
            var my = await GetAthlete(id);
            var matches = await SimilarAthletes(my.Summary);

            return new SimilarAthletesViewModel
            {
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                Athlete = my.Summary.Athlete,
                Matches = Rank(matches)
            };
        }

        private async Task<IEnumerable<SimilarAthlete>> SimilarAthletes(AthleteSummary my)
        {
            var myTotalResults = my.Fastest.Count(r => r.Value != null) + my.Average.Count(r => r.Value != null);
            var allResults = (await _dataService.GetAllResults()).ToList();

            var fastMatches = allResults.ToDictionary(c => c, c => c.Fastest().Where(r => my.Fastest[c] != null && r.Result.Athlete != my.Athlete && IsMatch(my.Fastest[c], r)));
            var avgMatches = allResults.ToDictionary(c => c, c => c.BestAverage().Where(r => my.Average[c] != null && r.Result.Athlete != my.Athlete && IsMatch(my.Average[c], r)));

            var athletes = fastMatches.SelectMany(c => c.Value.Select(r => r.Result.Athlete))
                .Union(avgMatches.SelectMany(c => c.Value.Select(r => r.Result.Athlete)))
                .Distinct();

            var matches = new List<SimilarAthlete>();
            foreach (var athlete in athletes)
            {
                var them = await GetAthlete(athlete.ID);
                var their = them.Summary;
                
                var fastestToCompare = my.Fastest.Where(r => r.Value != null && their.Fastest[r.Key] != null).ToList();
                var avgToCompare = my.Average.Where(r => r.Value != null && their.Average[r.Key] != null).ToList();
                var totalMatches = fastestToCompare.Count + avgToCompare.Count;
                
                var fastestDiffTotal = fastestToCompare.Sum(r => r.Value != null && their.Fastest[r.Key] != null ? Time.PercentDifference(r.Value.Value, their.Fastest[r.Key].Value) : 0) / totalMatches;
                var avgDiffTotal = avgToCompare.Sum(r => r.Value != null && their.Average[r.Key] != null ? Time.PercentDifference(r.Value.Value, their.Average[r.Key].Value) : 0) / totalMatches;
                
                var score = fastestToCompare.Sum(r => r.Value != null && their.Fastest[r.Key] != null ? 100 - Time.AbsolutePercentDifference(r.Value.Value, their.Fastest[r.Key].Value) / 2 : 0)
                    + avgToCompare.Sum(r => r.Value != null && their.Average[r.Key] != null ? 100 - Time.AbsolutePercentDifference(r.Value.Value, their.Average[r.Key].Value) / 2 : 0);

                matches.Add(new SimilarAthlete
                {
                    Athlete = athlete,
                    Similarity = new Percent(score  / totalMatches),
                    Confidence = new Percent(100.0 * totalMatches / myTotalResults),
                    FastestPercent = fastestToCompare.Any()
                        ? new SpeedComparison(fastestDiffTotal)
                        : null,
                    AveragePercent = avgToCompare.Any()
                        ? new SpeedComparison(avgDiffTotal)
                        : null
                });                
            }

            return matches;
        }

        private static IEnumerable<SimilarAthlete> Rank(IEnumerable<SimilarAthlete> matches)
        {
            var ordered = matches.OrderByDescending(r => r.Similarity.Value + r.Similarity.Value * r.Confidence.Value / 2).ToArray();
            for (var x = 0; x < ordered.Length; x++)
                ordered[x].Rank = new Rank((ushort)(x + 1));
            return ordered;
        }

        private const byte percentThreshold = 5;
        private static bool IsMatch(Ranked<Time> mine, Ranked<Time> theirs)
            => Time.AbsolutePercentDifference(mine.Value, theirs.Value) <= percentThreshold;

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
    }
}