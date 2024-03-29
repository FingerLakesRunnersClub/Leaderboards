using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class AthletesController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public AthletesController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index() => View(await GetAthletes());

	private async Task<AthletesViewModel> GetAthletes()
	{
		return new AthletesViewModel
		{
			Config = _config,
			Athletes = await _dataService.GetAthletes()
		};
	}
}