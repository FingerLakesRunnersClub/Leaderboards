using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Reports;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class InvalidController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public InvalidController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index() => View(await GetResults());

	private async Task<InvalidViewModel> GetResults()
	{
		var results = await _dataService.GetAllResults();
		var invalid = results.ToDictionary(c => c,
			c => c.GroupedResults()
				.Select(g => g.OrderBy(r => r.Duration).First())
				.Where(r => r.AgeGrade >= 100)
				.ToArray());

		return new InvalidViewModel
		{
			Config = _config,
			Results = invalid.Where(r => r.Value.Any())
				.ToDictionary(r => r.Key, r => r.Value)
		};
	}
}