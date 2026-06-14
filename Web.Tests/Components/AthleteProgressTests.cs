using System.Security.Claims;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Components;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class AthleteProgressTests
{
	private static readonly ClaimsPrincipal User =
		new (new ClaimsIdentity([new Claim("external_id", "123")]));

	[Fact]
	public async Task EmptyWhenNoActiveIteration()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();

		var athleteProgress = new AthleteProgress(iterationManager);

		iterationManager.ActiveIteration().Returns(new Iteration { EndDate = new DateOnly(2025, 12, 31) });

		//act
		var athlete = new Athlete();
		var result = await athleteProgress.InvokeAsync(athlete);

		//assert
		var content = result as ContentViewComponentResult;
		Assert.Empty(content!.Content);
	}

	[Fact]
	public async Task ShowsProgressWhenChallengeIsActive()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();

		var athleteProgress = new AthleteProgress(iterationManager);

		var iteration = new Iteration { RegistrationType = nameof(WebScorer) };
		iterationManager.ActiveIteration().Returns(iteration);

		var challenge = new Challenge { Iteration = iteration, IsPrimary = true };
		var athlete = new Athlete { Registrations = [iteration], Challenges = [challenge] };

		//act
		var result = await athleteProgress.InvokeAsync(athlete);

		//assert
		var content = result as ViewViewComponentResult;
		Assert.EndsWith("Progress", content!.ViewName);
	}
}