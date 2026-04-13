using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.Services;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Services;

public sealed class OverallResultsCalculatorTests
{
	[Fact]
	public void CanGetMostPoints()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var mostPoints = calculator.MostPoints();

		//assert
		Assert.Equal(ResultsData.Athlete1, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForCategory()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var mostPoints = calculator.MostPoints(new Filter(Category.F));

		//assert
		Assert.Equal(ResultsData.Athlete3, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForTopRaces()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var mostPoints = calculator.MostPoints(1);

		//assert
		Assert.Equal(ResultsData.Athlete1, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForCategoryTopRaces()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var mostPoints = calculator.MostPoints(1, new Filter(Category.F));

		//assert
		Assert.Equal(ResultsData.Athlete3, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostMiles()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var mostMiles = calculator.MostMiles();

		//assert
		Assert.Equal(ResultsData.Athlete2, mostMiles.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostMilesForCategory()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var mostMiles = calculator.MostMiles(new Filter(Category.F));

		//assert
		Assert.Equal(ResultsData.Athlete4, mostMiles.First().Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageAgeGrade()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var ageGrade = calculator.AgeGrade();

		//assert
		Assert.Equal(ResultsData.Athlete1, ageGrade.First().Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageAgeGradeForCategory()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var ageGrade = calculator.AgeGrade(new Filter(Category.F));

		//assert
		Assert.Equal(ResultsData.Athlete3, ageGrade.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCompleted()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var completed = calculator.Completed();

		//assert
		Assert.Equal(ResultsData.Athlete4, completed.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCompletedForCategory()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var completed = calculator.Completed(new Filter(Category.M));

		//assert
		Assert.Equal(ResultsData.Athlete1, completed.First().Result.Athlete);
	}

	[Fact]
	public void CanGetTeamPoints()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var teamPoints = calculator.TeamPoints();

		//assert
		Assert.Equal("1–29", teamPoints.First().Value.Team.Display);
	}

	[Fact]
	public void CanFilterTeamMembers()
	{
		//arrange
		var calculator = new OverallResultsCalculator(UltraChallengeData.Iteration);

		//act
		var members = calculator.TeamMembers(Team.Teams[2]);

		//assert
		Assert.Equal("1–29", members.First().Result.Athlete.Team(UltraChallengeData.Iteration).Display);
	}

	[Fact]
	public void PrivateAthletesInAllNonTimeBasedCompetitions()
	{
		//arrange
		var iteration = new Iteration
		{
			Races =
			[
				new Race
				{
					Courses =
					[
						new Course
						{
							Race = new Race { Name = "Test" },
							Distance = 10,
							Units = "mi",
							Results =
							[
								new Result
								{
									Course = new Course { Distance = 10, Units = "mi" },
									StartTime = new DateTime(2024, 04, 15, 9, 36, 00),
									Duration = TimeSpan.FromHours(2),
									Athlete = ResultsData.Private
								}
							]
						}
					]
				}
			]
		};

		//act
		var vm = new OverallResultsCalculator(iteration);

		//assert
		Assert.Empty(vm.MostPoints());
		Assert.Empty(vm.AgeGrade());
		Assert.NotEmpty(vm.MostMiles());
		Assert.NotEmpty(vm.TeamPoints());
		Assert.NotEmpty(vm.Completed());
		Assert.NotEmpty(vm.TeamMembers(new Team(4)));
	}
}