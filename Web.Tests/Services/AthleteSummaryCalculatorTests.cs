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
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var overall = new OverallResultsCalculator(starCalculator);
		var calculator = new AthleteSummaryCalculator(resultService, overall, starCalculator, TestHelpers.Config);

		var athlete1 = new Athlete { ID = Guid.NewGuid(), Category = 'M', DateOfBirth = new DateOnly(2000, 1, 1) };
		var athlete2 = new Athlete { ID = Guid.NewGuid(), Category = 'F', DateOfBirth = new DateOnly(2000, 1, 1) };

		var course = ResultsData.Course with { Distance = 10, Units = "mi" };

		var results = new[]
		{
			new Result
			{
				Athlete = athlete1,
				AthleteID = athlete1.ID,
				Course = course,
				StartTime = DateTime.Parse("1/1/2020"),
				Duration = new TimeSpan(2, 4, 6)
			},
			new Result
			{
				Athlete = athlete2,
				AthleteID = athlete2.ID,
				Course = course,
				StartTime = DateTime.Parse("1/1/2020"),
				Duration = new TimeSpan(1, 2, 3)
			}
		};

		var fullCourse = course with { Results = results };

		var iteration = new Iteration
		{
			Races = [fullCourse.Race with { Courses = [fullCourse]}],
			Challenges = [new Challenge { IsOfficial = true, IsPrimary = true, Courses = [fullCourse] }],
			StartDate = new DateOnly(2020, 1, 1)
		};

		resultService.Find(Arg.Any<Iteration>()).Returns(results);

		//act
		var summary = await calculator.GetSummary(athlete1, iteration);

		//assert
		Assert.Equal(athlete1.ID, summary.Athlete.ID);
		Assert.Equal(1, summary.Fastest[course].Rank.Value);
		Assert.Equal(1, summary.Average[course].Rank.Value);
		Assert.Equal(1, summary.Runs[course].Rank.Value);
		Assert.Equal(1, summary.TeamResults.Rank.Value);
		Assert.Equal(100, summary.OverallPoints.Value.Value);
		Assert.Equal(10, summary.OverallMiles.Value.Value);
	}

	[Fact]
	public async Task CanFindSimilarAthletes()
	{
		//arrange
		var resultService = Substitute.For<IResultService>();
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var overall = new OverallResultsCalculator(starCalculator);
		var athleteSummaryCalculator = new AthleteSummaryCalculator(resultService, overall, starCalculator, TestHelpers.Config);

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