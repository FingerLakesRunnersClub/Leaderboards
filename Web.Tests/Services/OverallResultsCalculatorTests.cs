using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.Services;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Services;

public sealed class OverallResultsCalculatorTests
{
	[Fact]
	public void CanGetMostPoints()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var mostPoints = calculator.MostPoints(OverallData.Iteration);

		//assert
		Assert.Equal(OverallData.Athlete1, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForCategory()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var mostPoints = calculator.MostPoints(OverallData.Iteration, new Filter(Category.F));

		//assert
		Assert.Equal(OverallData.Athlete3, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForTopRaces()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var mostPoints = calculator.MostPoints(OverallData.Iteration, 1);

		//assert
		Assert.Equal(OverallData.Athlete1, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForCategoryTopRaces()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var mostPoints = calculator.MostPoints(OverallData.Iteration, 1, new Filter(Category.F));

		//assert
		Assert.Equal(OverallData.Athlete3, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostMiles()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var mostMiles = calculator.MostMiles(OverallData.Iteration);

		//assert
		Assert.Equal(OverallData.Athlete2, mostMiles.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostMilesForCategory()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var mostMiles = calculator.MostMiles(OverallData.Iteration, new Filter(Category.F));

		//assert
		Assert.Equal(OverallData.Athlete4, mostMiles.First().Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageAgeGrade()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var ageGrade = calculator.AgeGrade(OverallData.Iteration);

		//assert
		Assert.Equal(OverallData.Athlete1, ageGrade.First().Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageAgeGradeForCategory()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var ageGrade = calculator.AgeGrade(OverallData.Iteration, new Filter(Category.F));

		//assert
		Assert.Equal(OverallData.Athlete3, ageGrade.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCompleted()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var iteration = OverallData.Iteration;
		var athletes = iteration.Races.SelectMany(r => r.Courses).SelectMany(c => c.Results).Select(r => r.Athlete).Distinct();
		var challenge = OverallData.OfficialChallenge with { Iteration = iteration };
		iteration.Challenges.Clear();
		iteration.Challenges.Add(challenge);
		foreach (var athlete in athletes)
			athlete.Challenges.Add(challenge);

		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var completed = calculator.Completed(iteration);

		//assert
		Assert.Equal(OverallData.Athlete4, completed.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCompletedForCategory()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var iteration = OverallData.Iteration;
		var athletes = iteration.Races.SelectMany(r => r.Courses).SelectMany(c => c.Results).Select(r => r.Athlete).Distinct();
		var challenge = OverallData.OfficialChallenge with { Iteration = iteration };
		iteration.Challenges.Clear();
		iteration.Challenges.Add(challenge);
		foreach (var athlete in athletes)
			athlete.Challenges.Add(challenge);

		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var completed = calculator.Completed(iteration, new Filter(Category.M));

		//assert
		Assert.Equal(OverallData.Athlete1, completed.First().Result.Athlete);
	}

	[Fact]
	public void CanGetTeamPoints()
	{
		//arrange
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var teamPoints = calculator.TeamPoints(OverallData.Iteration);

		//assert
		Assert.Equal("1–29", teamPoints.First().Value.Team.Display);
	}

	[Fact]
	public void CanFilterTeamMembers()
	{
		//arrange
		var iteration = OverallData.Iteration;
		var starCalculator = Substitute.For<ICommunityStarCalculator>();
		var calculator = new OverallResultsCalculator(starCalculator);

		//act
		var members = calculator.TeamMembers(iteration, Team.Teams[2]);

		//assert
		Assert.Equal("1–29", members.First().Result.Athlete.Team(iteration).Display);
	}

	[Fact]
	public void PrivateAthletesInAllNonTimeBasedCompetitions()
	{
		//arrange
		var athlete = OverallData.Private;
		var result = new Result
		{
			Course = new Course { Distance = 10, Units = "mi", Race = new Race { Type = "Road" } },
			StartTime = new DateTime(2024, 04, 15, 9, 36, 00),
			Duration = TimeSpan.FromHours(2),
			Athlete = athlete
		};
		var course = new Course
		{
			Race = new Race { Name = "Test" },
			Distance = 10,
			Units = "mi",
			Results = [result]
		};
		var race = new Race { Courses = [course] };
		var iteration = new Iteration
		{
			Races = [race],
			StartDate = new DateOnly(2020, 1, 1)
		};
		var challenge = new Challenge { IsOfficial = true, IsPrimary = true, Courses = [course], Iteration = iteration };
		athlete.Challenges.Add(challenge);
		iteration.Challenges.Add(challenge);

		var starCalculator = Substitute.For<ICommunityStarCalculator>();

		//act
		var calculator = new OverallResultsCalculator(starCalculator);

		//assert
		Assert.Empty(calculator.MostPoints(iteration));
		Assert.Empty(calculator.AgeGrade(iteration));
		Assert.NotEmpty(calculator.MostMiles(iteration));
		Assert.NotEmpty(calculator.Completed(iteration));
		Assert.NotEmpty(calculator.TeamPoints(iteration));
		Assert.NotEmpty(calculator.TeamMembers(iteration, new Team(4)));
	}
}