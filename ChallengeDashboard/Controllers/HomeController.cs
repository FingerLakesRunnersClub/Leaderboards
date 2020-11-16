using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ChallengeDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly CourseService _courseService;
        private readonly IEnumerable<uint> _courseIDs;

        public HomeController(IConfiguration configuration, CourseService courseService)
        {
            _courseService = courseService;
            _courseIDs = configuration.GetSection("Courses").AsEnumerable()
                .Where(id => !string.IsNullOrWhiteSpace(id.Value))
                .Select(id => Convert.ToUInt32(id.Value));
        }

        public async Task<IActionResult> Index() => View(await _courseService.All(_courseIDs));

        public async Task<IActionResult> Course(uint id) => View(await _courseService.GetCourse(id));
    }
}
