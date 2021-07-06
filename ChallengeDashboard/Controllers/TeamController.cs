using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class TeamController : Controller
    {
        private readonly IDataService _dataService;

        public TeamController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Index(byte id) => View(await GetTeam(Athlete.Teams[id]));

        public async Task<ViewResult> Members(byte id) => View(await GetMembers(Athlete.Teams[id]));

        private async Task<TeamMembersViewModel> GetMembers(Team team)
        {
            var overall = new OverallResults(await _dataService.GetAllResults());
            return new TeamMembersViewModel
            {
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                ResultType = "Members",
                Team = team,
                RankedResults = overall.TeamMembers(team.Value)
            };
        }

        private async Task<TeamSummaryViewModel> GetTeam(Team team)
        {
            var courses = (await _dataService.GetAllResults()).ToList();
            var overall = new OverallResults(courses);

            return new TeamSummaryViewModel
            {
                CourseNames = _dataService.CourseNames,
                Links = _dataService.Links,
                Team = team,
                Overall = overall.TeamPoints().FirstOrDefault(r => r.Team == team),
                Courses = courses.ToDictionary(c => c, c => c.TeamPoints().FirstOrDefault(r => r.Team == team))
            };
        }
    }
}