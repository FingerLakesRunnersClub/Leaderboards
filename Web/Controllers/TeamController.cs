using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class TeamController(IIterationManager iterationManager, ICommunityStarCalculator starCalculator) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index(byte id)
		=> View(await GetTeam(Team.Teams[id]));

	[HttpGet]
	public async Task<ViewResult> Members(byte id)
		=> View(await GetMembers(Team.Teams[id]));

	private async Task<ViewModel<TeamMembers>> GetMembers(Team team)
	{
		var iteration = await iterationManager.ActiveIteration();
		var overall = new OverallResultsCalculator(starCalculator, iteration);

		var results = new TeamMembers
		{
			Team = team,
			ResultType = "Members",
			RankedResults = overall.TeamMembers(team)
		};
		return new ViewModel<TeamMembers>($"Team {team.Display} Members", results);
	}

	private async Task<ViewModel<TeamSummary>> GetTeam(Team team)
	{
		var iteration = await iterationManager.ActiveIteration();
		var overall = new OverallResultsCalculator(starCalculator, iteration);

		var summary = new TeamSummary
		{
			Team = team,
			Overall = overall.TeamPoints().Find(r => r.Value.Team == team),
			Courses = iteration.OfficialChallenge.Courses.ToDictionary(c => c, c => c.Results.TeamPoints(iteration).Find(r => r.Value.Team == team))
		};
		return new ViewModel<TeamSummary>($"Team {team.Display}", summary);
	}
}