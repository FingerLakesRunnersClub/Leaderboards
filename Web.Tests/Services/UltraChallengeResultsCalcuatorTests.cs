using FLRC.Leaderboards.Web.Services;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Services;

public sealed class UltraChallengeResultsCalculatorTests
{
	[Fact]
	public async Task CanGetEarliestResults()
	{
		//arrange
		var iteration = UltraChallengeData.Iteration;

		//act
		var results = await UltraChallengeResultsCalculator.Earliest(iteration);

		//assert
		Assert.Equal(3, results.Count);

		var ultraResults = results[UltraChallengeData.OneHundredK];
		Assert.Equal(3, ultraResults.Count);
		Assert.Equal(ResultsData.Athlete2, ultraResults[0].Value.Athlete);
		Assert.Equal(ResultsData.Athlete1, ultraResults[1].Value.Athlete);
		Assert.Equal(ResultsData.Athlete1, ultraResults[2].Value.Athlete);
	}

	[Fact]
	public async Task CanGetFastestResults()
	{
		//arrange
		var iteration = UltraChallengeData.Iteration;

		//act
		var results = await UltraChallengeResultsCalculator.Fastest(iteration);

		//assert
		Assert.Equal(3, results.Count);

		var ultraResults = results[UltraChallengeData.OneHundredK];
		Assert.Equal(3, ultraResults.Count);
		Assert.Equal(ResultsData.Athlete1, ultraResults[0].Value.Athlete);
		Assert.Equal(ResultsData.Athlete2, ultraResults[1].Value.Athlete);
		Assert.Equal(ResultsData.Athlete1, ultraResults[2].Value.Athlete);
	}
}