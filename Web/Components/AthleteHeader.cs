using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FLRC.Leaderboards.Web.Components;

public sealed class AthleteHeader(IIterationManager iterationManager, IOverallResultsCalculator overall) : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync(Athlete athlete)
	{
		var iteration = await iterationManager.ActiveIteration();
		var badges = iteration is not null
			? await GetBadges(athlete, iteration)
			: null;

		var vm = new ViewModels.AthleteHeader { Athlete = athlete, Badges = badges };
		return View("../AthleteHeader", vm);
	}

	private async Task<IDictionary<string, string>> GetBadges(Athlete athlete, Iteration iteration)
	{
		var completed = overall.Completed(iteration).Any(r => r.Result.Athlete == athlete);

		var challengeResults = await UltraChallengeResultsCalculator.Earliest(iteration);
		var result = challengeResults
			.Where(s => s.Value.Any(r => r.Value.Athlete == athlete))
			.ToDictionary(s => BadgeIcon(s.Key), s => s.Key.Name);

		return completed
			? result.Prepend(new KeyValuePair<string, string>("medal", iteration.OfficialChallenge?.Name ?? "Challenge Completed")).ToDictionary(d => d.Key, d => d.Value)
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