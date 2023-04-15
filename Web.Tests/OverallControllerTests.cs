using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Core.Tests.Leaders;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public class OverallControllerTests
{
	[Fact]
	public async Task CanGetMostPoints()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Points(Category.M.Display);

		//assert
		var vm = (OverallResultsViewModel<Points>) response.Model;
		Assert.Equal(LeaderboardData.Athlete1, vm!.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetMostPointsForLimitedEvents()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService, TestHelpers.Config);

		//act
		var response = await controller.PointsTop3(Category.M.Display);

		//assert
		var vm = (OverallResultsViewModel<Points>) response.Model;
		Assert.Equal(LeaderboardData.Athlete1, vm!.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetMostMiles()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Miles();

		//assert
		var vm = (OverallResultsViewModel<Miles>) response.Model;
		Assert.Equal(LeaderboardData.Athlete2, vm!.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetBestAgeGradeAverage()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService, TestHelpers.Config);

		//act
		var response = await controller.AgeGrade();

		//assert
		var vm = (OverallResultsViewModel<AgeGrade>) response.Model;
		Assert.Equal(LeaderboardData.Athlete1, vm!.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetCommunityStars()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Community();

		//assert
		var vm = (OverallResultsViewModel<Stars>) response.Model;
		Assert.Equal(LeaderboardData.Athlete4, vm!.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetTeamPoints()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new OverallController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Team();

		//assert
		var vm = (OverallResultsViewModel<TeamResults>) response.Model;
		Assert.Equal("1–29", vm!.RankedResults.First().Value.Team.Display);
	}
}