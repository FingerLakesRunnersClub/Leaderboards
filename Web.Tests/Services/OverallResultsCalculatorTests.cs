using FLRC.Leaderboards.Core.Athletes;
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
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var mostPoints = calculator.MostPoints();

		//assert
		Assert.Equal(OverallData.Athlete1, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForCategory()
	{
		//arrange
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var mostPoints = calculator.MostPoints(new Filter(Category.F));

		//assert
		Assert.Equal(OverallData.Athlete3, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForTopRaces()
	{
		//arrange
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var mostPoints = calculator.MostPoints(1);

		//assert
		Assert.Equal(OverallData.Athlete1, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForCategoryTopRaces()
	{
		//arrange
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var mostPoints = calculator.MostPoints(1, new Filter(Category.F));

		//assert
		Assert.Equal(OverallData.Athlete3, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostMiles()
	{
		//arrange
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var mostMiles = calculator.MostMiles();

		//assert
		Assert.Equal(OverallData.Athlete2, mostMiles.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostMilesForCategory()
	{
		//arrange
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var mostMiles = calculator.MostMiles(new Filter(Category.F));

		//assert
		Assert.Equal(OverallData.Athlete4, mostMiles.First().Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageAgeGrade()
	{
		//arrange
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var ageGrade = calculator.AgeGrade();

		//assert
		Assert.Equal(OverallData.Athlete1, ageGrade.First().Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageAgeGradeForCategory()
	{
		//arrange
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var ageGrade = calculator.AgeGrade(new Filter(Category.F));

		//assert
		Assert.Equal(OverallData.Athlete3, ageGrade.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCompleted()
	{
		//arrange
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var completed = calculator.Completed();

		//assert
		Assert.Equal(OverallData.Athlete4, completed.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCompletedForCategory()
	{
		//arrange
		var calculator = new OverallResultsCalculator(OverallData.Iteration);

		//act
		var completed = calculator.Completed(new Filter(Category.M));

		//assert
		Assert.Equal(OverallData.Athlete1, completed.First().Result.Athlete);
	}

	[Fact]
	public void PrivateAthletesInAllNonTimeBasedCompetitions()
	{
		//arrange
		var result = new Result
		{
			Course = new Course { Distance = 10, Units = "mi", Race = new Race { Type = "Road" }},
			StartTime = new DateTime(2024, 04, 15, 9, 36, 00),
			Duration = TimeSpan.FromHours(2),
			Athlete = OverallData.Private
		};
		var course = new Course
		{
			Race = new Race { Name = "Test" },
			Distance = 10,
			Units = "mi",
			Results = [result]
		};
		var challenge = new Challenge { IsOfficial = true, IsPrimary = true, Courses = [course] };
		var race = new Race { Courses = [course] };
		var iteration = new Iteration
		{
			Challenges = [challenge],
			Races = [race]
		};

		//act
		var calculator = new OverallResultsCalculator(iteration);

		//assert
		Assert.Empty(calculator.MostPoints());
		Assert.Empty(calculator.AgeGrade());
		Assert.NotEmpty(calculator.MostMiles());
		Assert.NotEmpty(calculator.Completed());
	}
}