using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class CourseController : Controller
    {
        private readonly IDataService _dataService;

        public CourseController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Fastest(uint id, string cat = null)
        {
            var category = DataParser.ParseCategory(cat);
            return View(await GetResults(id, ResultType.Fastest, category, c => c.Fastest(category)));
        }

        public async Task<ViewResult> MostRuns(uint id, string cat = null)
        {
            var category = DataParser.ParseCategory(cat);
            return View(await GetResults(id, ResultType.MostRuns, category, c => c.MostRuns(category)));
        }

        public async Task<ViewResult> BestAverage(uint id, string cat = null)
        {
            var category = DataParser.ParseCategory(cat);
            return View(await GetResults(id, ResultType.BestAverage, category, c => c.BestAverage(category)));
        }

        public async Task<ViewResult> Team(uint id) => View(await GetTeamResults(id));

        private async Task<TeamResultsViewModel> GetTeamResults(uint courseID)
        {
            var course = await _dataService.GetResults(courseID);
            return new TeamResultsViewModel
            {
                CourseNames = _dataService.CourseNames,
                ResultType = new FormattedResultType(ResultType.Team),
                Course = course,
                Results = course.TeamPoints()
            };
        }

        private async Task<ResultsViewModel<T>> GetResults<T>(uint courseID, ResultType resultType, Category category, Func<Course, RankedList<T>> results)
        {
            var course = await _dataService.GetResults(courseID);
            return new ResultsViewModel<T>
            {
                CourseNames = _dataService.CourseNames,
                ResultType = new FormattedResultType(resultType),
                Category = category,
                Course = course,
                RankedResults = results(course)
            };
        }
    }
}
