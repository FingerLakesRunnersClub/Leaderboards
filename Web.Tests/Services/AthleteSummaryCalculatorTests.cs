using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Services;

public sealed class AthleteSummaryCalculatorTests
{
	[Fact]
	public async Task SummaryContainsAllInfo()
	{
		//arrange
		var resultService = Substitute.For<IResultService>();
		var calculator = new AthleteSummaryCalculator(resultService, TestHelpers.Config);

		var athlete = new Athlete { ID = Guid.NewGuid(), Category = 'M' };
		var course = ResultsData.Course with { Distance = 10, Units = "mi" };
		var results = new[]
		{
			new Result
			{
				Athlete = athlete,
				Course = course,
				StartTime = DateTime.Parse("1/1/2000"),
				Duration = new TimeSpan(2, 4, 6)
			},
			new Result
			{
				Athlete = new Athlete { ID = Guid.NewGuid(), Category = 'F' },
				Course = course,
				StartTime = DateTime.Parse("1/1/2000"),
				Duration = new TimeSpan(1, 2, 3)
			}
		};
		var fullCourse = course with { Results = results };
		var iteration = new Iteration
		{
			Races = [new Race { Courses = [fullCourse] }],
			Challenges = [new Challenge { IsOfficial = true, IsPrimary = true, Courses = [fullCourse]}]
		};

		resultService.Find(Arg.Any<Iteration>()).Returns(results);

		//act
		var summary = await calculator.GetSummary(athlete, iteration);

		//assert
		Assert.Equal(athlete.ID, summary.Athlete.ID);
		Assert.Equal(1, summary.Fastest[course].Rank.Value);
		Assert.Equal(1, summary.Average[course].Rank.Value);
		Assert.Equal(1, summary.Runs[course].Rank.Value);
		Assert.Equal(100, summary.OverallPoints.Value.Value);
		Assert.Equal(10, summary.OverallMiles.Value.Value);
	}

	[Fact]
	public async Task CanFindSimilarAthletes()
	{
		//arrange
		var resultService = Substitute.For<IResultService>();
		var athleteSummaryCalculator = new AthleteSummaryCalculator(resultService, TestHelpers.Config);

		resultService.Find(Arg.Any<Iteration>()).Returns(ResultsData.SimilarResults);

		//act
		var summary = await athleteSummaryCalculator.GetSummary(ResultsData.Athlete1, new Iteration());
		var matches = await athleteSummaryCalculator.SimilarAthletes(summary);

		//assert
		Assert.Equal(ResultsData.Athlete4, matches[0].Athlete);
		Assert.Equal("96%", matches[0].Similarity.Display);
		Assert.Equal(ResultsData.Athlete2, matches[1].Athlete);
		Assert.Equal("95%", matches[1].Similarity.Display);
	}
}