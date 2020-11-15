using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ChallengeDashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly CourseService _courseService;

        public HomeController(IConfiguration configuration, CourseService courseService)
        {
            _configuration = configuration;
            _courseService = courseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IEnumerable<Course>> Courses()
        {
            var tasks = _configuration.GetValue<string[]>("Courses")
                .Select(id => _courseService.GetCourse(id))
                .ToArray();
            await Task.WhenAll(tasks);
            return tasks.Select(t => t.GetAwaiter().GetResult());
        }

        public async Task<Course> Course(uint id)
        {
            return await _courseService.GetCourse(id);
        }
    }
}
