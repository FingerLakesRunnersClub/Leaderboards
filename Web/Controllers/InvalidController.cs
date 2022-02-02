using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Reports;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class InvalidController : Controller
{
	private readonly IDataService _dataService;

	public InvalidController(IDataService dataService) => _dataService = dataService;

	public async Task<ViewResult> Index() => View(await GetResults());

	private async Task<InvalidViewModel> GetResults()
	{
		var results = await _dataService.GetAllResults();
		var invalid = results.ToDictionary(c => c,
			c => c.GroupedResults()
				.Select(g => g.OrderBy(r => r.Duration).First())
				.Where(r => AgeGradeCalculator.AgeGradeCalculator.GetAgeGrade(r.Athlete.Category?.Value ?? Category.M.Value, r.Athlete.Age, c.Meters, r.Duration.Value) >= 100));

		return new InvalidViewModel
		{
			CourseNames = _dataService.CourseNames,
			Links = _dataService.Links,
			Results = invalid.Where(r => r.Value.Any()).ToDictionary(r => r.Key, r => r.Value)
		};
	}
}