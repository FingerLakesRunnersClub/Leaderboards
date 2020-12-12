using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDataService _dataService;
        private readonly IEnumerable<uint> _courseIDs;

        public DashboardController(IConfiguration configuration, IDataService dataService)
        {
            _dataService = dataService;
            _courseIDs = configuration.GetSection("Courses").AsEnumerable()
                .Where(id => !string.IsNullOrWhiteSpace(id.Value))
                .Select(id => Convert.ToUInt32(id.Value));
        }

        public async Task<IActionResult> Index() => View(await _dataService.GetAllCourses(_courseIDs));
    }
}
