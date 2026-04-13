using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Controllers;

public sealed class TeamController(IIterationManager iterationManager) : Controller
{
	[HttpGet]
	public async Task<ViewResult> Index(byte id)
		=> View(await GetTeam(Team.Teams[id]));

	private async Task<ViewModel<TeamSummary>> GetTeam(Team team)
	{
		var iteration = await iterationManager.ActiveIteration();
		var calculator = new OverallResultsCalculator(iteration);
		var overall = calculator.TeamPoints().Find(r => r.Value.Team == team);
		var courses = iteration.Races.SelectMany(r => r.Courses).ToDictionary(c => c, c => c.Results.TeamPoints(iteration).Find(r => r.Value.Team == team));

		var summary = new TeamSummary
		{
			Team = team,
			Overall = overall,
			Courses = courses
		};
		return new ViewModel<TeamSummary>($"Team {team.Display}", summary);
	}

	[HttpGet]
	public async Task<ViewResult> Members(byte id)
		=> View(await GetMembers(Team.Teams[id]));

	private async Task<ViewModel<TeamMembers>> GetMembers(Team team)
	{
		var iteration = await iterationManager.ActiveIteration();
		var calculator = new OverallResultsCalculator(iteration);

		var members = new TeamMembers
		{
			Team = team,
			Results = calculator.TeamMembers(team)
		};
		return new ViewModel<TeamMembers>($"Team {team.Display} Members", members);
	}
}