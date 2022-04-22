using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Reports;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class AwardsController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public AwardsController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index() => View(await GetAwards());

	private async Task<AwardsViewModel> GetAwards()
		=> new(_config, await _dataService.GetAllResults());
}
