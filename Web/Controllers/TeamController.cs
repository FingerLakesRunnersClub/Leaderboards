using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Teams;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class TeamController : Controller
{
	private readonly IDataService _dataService;
	private readonly IConfig _config;

	public TeamController(IDataService dataService, IConfig config)
	{
		_dataService = dataService;
		_config = config;
	}

	public async Task<ViewResult> Index(byte id) => View(await GetTeam(Athlete.Teams[id]));

	public async Task<ViewResult> Members(byte id) => View(await GetMembers(Athlete.Teams[id]));

	private async Task<TeamMembersViewModel> GetMembers(Team team)
	{
		var overall = new OverallResults(await _dataService.GetAllResults());
		return new TeamMembersViewModel
		{
			Config = _config,
			ResultType = "Members",
			Team = team,
			RankedResults = overall.TeamMembers(team)
		};
	}

	private async Task<TeamSummaryViewModel> GetTeam(Team team)
	{
		var courses = await _dataService.GetAllResults();
		var overall = new OverallResults(courses);

		return new TeamSummaryViewModel
		{
			Config = _config,
			Team = team,
			Overall = overall.TeamPoints().FirstOrDefault(r => r.Value.Team == team),
			Courses = courses.ToDictionary(c => c, c => c.TeamPoints().FirstOrDefault(r => r.Value.Team == team))
		};
	}
}