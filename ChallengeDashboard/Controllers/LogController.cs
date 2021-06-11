using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class LogController : Controller
    {
        private readonly IDataService _dataService;
        private readonly DateTime _today;

        public LogController(IDataService dataService, DateTime? today = null)
        {
            _dataService = dataService;
            _today = today ?? DateTime.Today;
        }

        public async Task<ViewResult> Index()
            => View(await GetActivityLog(ActivityLogType.Recent, r => getGroup(r.StartTime.Value.Date), _today.Subtract(TimeSpan.FromDays(7))));

        public async Task<ViewResult> All()
            => View("Index", await GetActivityLog(ActivityLogType.Archive, r => getMonth(r.StartTime.Value.Date)));

        private async Task<ActivityLogViewModel> GetActivityLog(ActivityLogType type, Func<Result, string> grouping, DateTime? filter = null)
            => new()
            {
                Type = type,
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                Results = (await _dataService.GetAllResults())
                    .SelectMany(c => c.Results)
                    .Where(r => filter == null || r.StartTime.Value.Date > filter)
                    .OrderByDescending(r => r.StartTime.Value)
                    .GroupBy(grouping)
            };

        private static string getGroup(DateTime date)
            => date == DateTime.Today ? "Today"
                : date == DateTime.Today.Subtract(TimeSpan.FromDays(1)) ? "Yesterday"
                : "This Week";

        private static string getMonth(DateTime date)
            => date.ToString("MMMM");
    }
}