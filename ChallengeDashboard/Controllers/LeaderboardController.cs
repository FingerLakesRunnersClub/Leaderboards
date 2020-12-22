using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly IDataService _dataService;

        public LeaderboardController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Index() => View(await GetLeaderboard());

        private async Task<LeaderboardViewModel> GetLeaderboard()
        {
            var results = await _dataService.GetAllResults();

            return new LeaderboardViewModel(results);
        }
    }
}
