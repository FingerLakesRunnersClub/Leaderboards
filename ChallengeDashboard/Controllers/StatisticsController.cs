using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IDataService _dataService;

        public StatisticsController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Index() => View(await GetStatistics());

        private async Task<StatisticsViewModel> GetStatistics()
        {
            var courseStats = (await _dataService.GetAllResults())
                .ToDictionary(c => c, c => c.Statistics());
            var athletes = courseStats.SelectMany(stats => stats.Key.Results.Select(r => r.Athlete)).Distinct()
                .ToList();

            return new()
            {
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                Courses = courseStats,
                Total = new Statistics
                {
                    Participants = new Dictionary<string, int>
                    {
                        {string.Empty, athletes.Count},
                        {Category.F.Display, athletes.Count(a => a.Category != null && a.Category.Equals(Category.F))},
                        {Category.M.Display, athletes.Count(a => a.Category != null && a.Category.Equals(Category.M))},
                    },
                    Runs = new Dictionary<string, int>
                    {
                        {string.Empty, courseStats.Sum(stats => stats.Value.Runs[string.Empty])},
                        {Category.F.Display, courseStats.Sum(stats => stats.Value.Runs[Category.F.Display])},
                        {Category.M.Display, courseStats.Sum(stats => stats.Value.Runs[Category.M.Display])},
                    },
                    Miles = new Dictionary<string, double>
                    {
                        {string.Empty, courseStats.Sum(stats => stats.Value.Miles[string.Empty])},
                        {Category.F.Display, courseStats.Sum(stats => stats.Value.Miles[Category.F.Display])},
                        {Category.M.Display, courseStats.Sum(stats => stats.Value.Miles[Category.M.Display])},
                    },
                    Average = new Dictionary<string, double>
                    {
                        {string.Empty, (double)courseStats.Sum(stats => stats.Value.Runs[string.Empty]) / athletes.Count},
                        {Category.F.Display, (double)courseStats.Sum(stats => stats.Value.Runs[Category.F.Display]) / athletes.Count(a => a.Category != null && a.Category.Equals(Category.F))},
                        {Category.M.Display, (double)courseStats.Sum(stats => stats.Value.Runs[Category.M.Display]) / athletes.Count(a => a.Category != null && a.Category.Equals(Category.M))},
                    }
                }
            };
        }
    }
}