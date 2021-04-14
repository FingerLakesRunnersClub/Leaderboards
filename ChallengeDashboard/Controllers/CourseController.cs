using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class CourseController : Controller
    {
        private readonly IDataService _dataService;

        public CourseController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Fastest(uint id, string other = null, byte? ag = null)
        {
            if (ag != null)
                ViewBag.AgeGroup = ag;
            var category = DataParser.ParseCategory(other);
            return View(await GetResults(id, ResultType.Fastest, category, c => c.Fastest(category, ag)));
        }

        public async Task<ViewResult> MostRuns(uint id, string other = null)
        {
            var category = DataParser.ParseCategory(other);
            return View(await GetResults(id, ResultType.MostRuns, category, c => c.MostRuns(category)));
        }

        public async Task<ViewResult> BestAverage(uint id, string other = null)
        {
            var category = DataParser.ParseCategory(other);
            return View(await GetResults(id, ResultType.BestAverage, category, c => c.BestAverage(category)));
        }

        public async Task<ViewResult> Team(uint id) => View(await GetTeamResults(id));

        private async Task<CourseTeamResultsViewModel> GetTeamResults(uint courseID)
        {
            var course = await _dataService.GetResults(courseID);
            return new CourseTeamResultsViewModel
            {
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                ResultType = new FormattedResultType(ResultType.Team),
                Course = course,
                Results = course.TeamPoints(),
            };
        }

        private async Task<CourseResultsViewModel<T>> GetResults<T>(uint courseID, ResultType resultType, Category category, Func<Course, RankedList<T>> results)
        {
            var course = await _dataService.GetResults(courseID);
            return new CourseResultsViewModel<T>
            {
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                ResultType = new FormattedResultType(resultType),
                Category = category,
                Course = course,
                RankedResults = results(course),
            };
        }
    }
}
