using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Groups;
using FLRC.Leaderboards.Core.Overall;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public class GroupController : Controller
{
	private readonly IDataService _dataService;

	public GroupController(IDataService dataService) => _dataService = dataService;

	public async Task<ViewResult> Index(string id) => View(await GetGroupResults(id));

	private async Task<GroupResultsViewModel> GetGroupResults(string id)
	{
		var group = await _dataService.GetGroupMembers(id);
		var results = await _dataService.GetAllResults();
		var overall = new OverallResults(results);

		return new GroupResultsViewModel
		{
			CourseNames = _dataService.CourseNames,
			Links = _dataService.Links,
			ResultType = "Members",
			Name = id,
			RankedResults = overall.GroupMembers(group)
		};
	}
}