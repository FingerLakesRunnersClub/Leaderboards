using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Components;

public sealed class AthleteHeader(IIterationManager iterationManager, IAthleteService athleteService) : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync(Guid id)
	{
		var iteration = await iterationManager.ActiveIteration();
		var athlete = await athleteService.Get(id);
		var badges = await GetBadges(athlete, iteration);

		var vm = new ViewModels.AthleteHeader { Athlete = athlete, Badges = badges };
		return View("../AthleteHeader", vm);
	}

	private async Task<IDictionary<string, string>> GetBadges(Athlete athlete, Iteration iteration)
	{
		var overall = new OverallResultsCalculator(iteration);
		var completed = overall.Completed().Any(r => r.Result.Athlete == athlete);

		var challengeResults = await UltraChallengeResultsCalculator.Earliest(iteration);
		var result = challengeResults
			.Where(s => s.Value.Any(r => r.Value.Athlete == athlete))
			.ToDictionary(s => BadgeIcon(s.Key), s => s.Key.Name);

		return completed
			? result.Prepend(new KeyValuePair<string, string>("medal", iteration.OfficialChallenge.Name)).ToDictionary(d => d.Key, d => d.Value)
			: result;
	}

	private static string BadgeIcon(Challenge challenge)
		=> challenge switch
		{
			{ IsOfficial: true, IsPrimary: true } => "medal",
			{ IsOfficial: true, IsPrimary: false } when challenge.Name.Contains("Tarmac") => "road",
			{ IsOfficial: true, IsPrimary: false } when challenge.Name.Contains("Trail") => "mountain",
			{ IsOfficial: true, IsPrimary: false } when challenge.Name.Contains("Ultra") => "diamond",
			_ => throw new ArgumentOutOfRangeException($"Invalid challenge {challenge.Name}")
		};
}