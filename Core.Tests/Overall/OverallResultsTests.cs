using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Tests.Leaders;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Overall;

public class OverallResultsTests
{
	[Fact]
	public void CanGetMostPoints()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var mostPoints = vm.MostPoints();

		//assert
		Assert.Equal(LeaderboardData.Athlete1, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForCategory()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var mostPoints = vm.MostPoints(Category.F);

		//assert
		Assert.Equal(LeaderboardData.Athlete3, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostMiles()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var mostMiles = vm.MostMiles();

		//assert
		Assert.Equal(LeaderboardData.Athlete2, mostMiles.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostMilesForCategory()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var mostMiles = vm.MostMiles(Category.F);

		//assert
		Assert.Equal(LeaderboardData.Athlete4, mostMiles.First().Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageAgeGrade()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var ageGrade = vm.AgeGrade();

		//assert
		Assert.Equal(LeaderboardData.Athlete1, ageGrade.First().Result.Athlete);
	}

	[Fact]
	public void CanGetBestAverageAgeGradeForCategory()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var ageGrade = vm.AgeGrade(Category.F);

		//assert
		Assert.Equal(LeaderboardData.Athlete3, ageGrade.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCommunityPoints()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var points = vm.CommunityPoints();

		//assert
		Assert.Equal(LeaderboardData.Athlete1, points.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCommunityPointsForCategory()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var points = vm.CommunityPoints(Category.F);

		//assert
		Assert.Equal(LeaderboardData.Athlete3, points.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCompleted()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var completed = vm.Completed();

		//assert
		Assert.Equal(LeaderboardData.Athlete4, completed.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCompletedForCategory()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var completed = vm.Completed(Category.M);

		//assert
		Assert.Equal(LeaderboardData.Athlete1, completed.First().Result.Athlete);
	}

	[Fact]
	public void CanGetTeamPoints()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var teamPoints = vm.TeamPoints();

		//assert
		Assert.Equal("1–29", teamPoints.First().Team.Display);
	}

	[Fact]
	public void CanFilterTeamMembers()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var members = vm.TeamMembers(2);

		//assert
		Assert.Equal("1–29", members.First().Result.Athlete.Team.Display);
	}

	[Fact]
	public void CanFilterGroupMembers()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var members = vm.GroupMembers(new[] { LeaderboardData.Athlete2 });

		//assert
		Assert.Equal(LeaderboardData.Athlete2, members.Single().Result.Athlete);
	}
}