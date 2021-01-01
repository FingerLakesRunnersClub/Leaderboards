using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard
{
    public class TeamController : Controller
    {
        private readonly IDataService _dataService;

        public TeamController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Index(byte id) => View(await GetTeam(Athlete.Teams[id]));

        private async Task<TeamSummaryViewModel> GetTeam(Team team)
        {
            var courses = await _dataService.GetAllResults();
            var overall = new OverallViewModel(courses);

            return new TeamSummaryViewModel
            {
                Team = team,
                CourseNames = _dataService.CourseNames,
                Overall = overall.TeamPoints().FirstOrDefault(r => r.Team == team),
                Courses = courses.ToDictionary(c => c, c => c.TeamPoints().FirstOrDefault(r => r.Team == team))
            };
        }
    }
}