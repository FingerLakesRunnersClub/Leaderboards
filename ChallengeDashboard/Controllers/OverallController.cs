using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class OverallController : Controller
    {
        private readonly IDataService _dataService;

        public OverallController(IDataService dataService) => _dataService = dataService;

        public async Task<ViewResult> Points(string cat = null)
        {
            var category = DataParser.ParseCategory(cat);
            return View(await GetResults($"Most Points ({cat})", vm => vm.MostPoints(category)));
        }

        public async Task<ViewResult> Miles()
            => View(await GetResults("Most Miles", vm => vm.MostMiles()));

        public async Task<ViewResult> Team()
            => View(await GetTeamResults("Team Points", vm => vm.TeamPoints()));

        private async Task<OverallResultsViewModel<T>> GetResults<T>(string title, Func<OverallViewModel, RankedList<T>> results)
        {
            var allResults = await _dataService.GetAllResults();
            var overallViewModel = new OverallViewModel(allResults);
            return new OverallResultsViewModel<T>
            {
                CourseNames = _dataService.CourseNames,
                ResultType = title,
                RankedResults = results(overallViewModel)
            };
        }
        
        private async Task<OverallTeamResultsViewModel> GetTeamResults(string title, Func<OverallViewModel, IEnumerable<TeamResults>> results)
        {
            var allResults = await _dataService.GetAllResults();
            var overallViewModel = new OverallViewModel(allResults);
            return new OverallTeamResultsViewModel
            {
                CourseNames = _dataService.CourseNames,
                ResultType = title,
                Results = results(overallViewModel)
            };
        }
    }
}