using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class LogController : Controller
    {
        private readonly IDataService _dataService;

        public LogController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Index() => View(await GetActivityLog());

        private async Task<ActivityLogViewModel> GetActivityLog() =>
            new()
            {
                CourseNames = _dataService.CourseNames,
                Results = (await _dataService.GetAllResults()).SelectMany(c => c.Results)
                    .OrderByDescending(r => r.StartTime.Value)
            };
    }
}