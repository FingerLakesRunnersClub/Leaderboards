using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Services;

public sealed class SimilarAthletesCalculatorTests
{
	[Fact]
	public async Task CanFindSimilarAthletes()
	{
		//arrange
		var resultService = Substitute.For<IResultService>();
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var overall = new OverallResultsCalculator(starCalculator);
		var calculator = new AthleteSummaryCalculator(resultService, overall, starCalculator, TestHelpers.Config);

		resultService.Find(Arg.Any<Iteration>()).Returns(ResultsData.SimilarResults);

		var my = await calculator.GetSummary(ResultsData.Athlete1, new Iteration());
		var their = await calculator.GetSummary(ResultsData.Athlete4, new Iteration());

		//act
		var similar = SimilarAthleteCalculator.GetSimilarity(my, their);

		//assert
		Assert.Equal("96%", similar.Similarity.Display);
	}
}