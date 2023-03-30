using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;
using FLRC.Leaderboards.Core.Teams;
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
		var mostPoints = vm.MostPoints(Filter.F);

		//assert
		Assert.Equal(LeaderboardData.Athlete3, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForTopRaces()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var mostPoints = vm.MostPoints(1);

		//assert
		Assert.Equal(LeaderboardData.Athlete1, mostPoints.First().Result.Athlete);
	}

	[Fact]
	public void CanGetMostPointsForCategoryTopRaces()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var mostPoints = vm.MostPoints(1, Filter.F);

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
		var mostMiles = vm.MostMiles(Filter.F);

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
		var ageGrade = vm.AgeGrade(Filter.F);

		//assert
		Assert.Equal(LeaderboardData.Athlete3, ageGrade.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCommunityStars()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var points = vm.CommunityStars();

		//assert
		Assert.Equal(LeaderboardData.Athlete1, points.First().Result.Athlete);
	}

	[Fact]
	public void CanGetCommunityStarsForCategory()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var points = vm.CommunityStars(Filter.F);

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
		var completed = vm.Completed(Filter.M);

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
		Assert.Equal("1–29", teamPoints.First().Value.Team.Display);
	}

	[Fact]
	public void CanFilterTeamMembers()
	{
		//arrange
		var vm = new OverallResults(LeaderboardData.Courses);

		//act
		var members = vm.TeamMembers(Athlete.Teams[2]);

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

	[Fact]
	public void PrivateAthletesInAllNonTimeBasedCompetitions()
	{
		//arrange
		var results = new[]
		{
			new Course
			{
				Race = new Race { Name = "Test" },
				Distance = new Distance("10 miles"),
				Results = new[]
				{
					new Result
					{
						Course = new Course { Distance = new Distance("10 miles") },
						Athlete = LeaderboardData.Private, CommunityStars = { [StarType.Story] = true }
					}
				}
			}
		};

		//act
		var vm = new OverallResults(results);

		//assert
		Assert.Empty(vm.MostPoints());
		Assert.Empty(vm.AgeGrade());
		Assert.NotEmpty(vm.MostMiles());
		Assert.NotEmpty(vm.TeamPoints());
		Assert.NotEmpty(vm.CommunityStars());
		Assert.NotEmpty(vm.Completed());
		Assert.NotEmpty(vm.TeamMembers(new Team(4)));
	}
}