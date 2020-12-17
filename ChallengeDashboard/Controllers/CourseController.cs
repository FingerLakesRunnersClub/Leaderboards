using FLRC.AgeGradeCalculator;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class CourseController : Controller
    {
        private readonly IDataService _dataService;

        public CourseController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Fastest(uint id, Category? category = null)
        {
            ViewBag.CourseNames = _dataService.CourseNames;
            return View(await GetResults(id, ResultType.Fastest, category, c => c.Fastest(category)));
        }

        public async Task<ViewResult> MostRuns(uint id, Category? category = null)
        {
            ViewBag.CourseNames = _dataService.CourseNames;
            return View(await GetResults(id, ResultType.MostRuns, category, c => c.MostRuns(category)));
        }

        public async Task<ViewResult> BestAverage(uint id, Category? category = null)
        {
            ViewBag.CourseNames = _dataService.CourseNames;
            return View(await GetResults(id, ResultType.BestAverage, category, c => c.BestAverage(category)));
        }

        private async Task<ResultsViewModel<T>> GetResults<T>(uint courseID, ResultType resultType, Category? category, Func<Course, RankedList<T>> results)
        {
            var course = await _dataService.GetResults(courseID);
            return new ResultsViewModel<T>
            {
                EntityType = EntityType.Athlete,
                ResultType = resultType,
                Category = category,
                Course = course,
                RankedResults = results(course)
            };
        }
    }
}
