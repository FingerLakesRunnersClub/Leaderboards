using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Overall;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class GroupController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public GroupController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index(string id) => View(await GetGroupResults(id));

	private async Task<GroupResultsViewModel> GetGroupResults(string id)
	{
		var group = await _dataService.GetGroupMembers(id);
		var results = await _dataService.GetAllResults();
		var overall = new OverallResults(results);

		return new GroupResultsViewModel
		{
			Config = _config,
			ResultType = "Members",
			Name = id,
			RankedResults = overall.GroupMembers(group)
		};
	}
}