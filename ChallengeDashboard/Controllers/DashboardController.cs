using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDataService _dataService;

        public DashboardController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Index() => View(await GetDashboard());

        private async Task<DashboardViewModel> GetDashboard()
        {
            var results = await _dataService.GetAllResults();

            return new DashboardViewModel(results);
        }
    }
}
