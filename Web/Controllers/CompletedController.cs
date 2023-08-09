using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Reports;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class CompletedController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public CompletedController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index() => View(await GetCompleted());

	private async Task<CompletedViewModel> GetCompleted()
	{
		var results = await _dataService.GetAllResults();
		var overall = new OverallResults(results);
		var personal = await _dataService.GetPersonalCompletions();

		return new CompletedViewModel
		{
			Config = _config,
			RankedResults = overall.Completed(),
			PersonalResults = personal
		};
	}
}