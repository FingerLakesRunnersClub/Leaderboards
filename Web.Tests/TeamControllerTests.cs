using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Core.Tests.Leaders;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web;

public class TeamControllerTests
{
	[Fact]
	public async Task CanGetTeamNameFromViewModel()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var controller = new TeamController(dataService);

		//act
		var response = await controller.Index(3);

		//assert
		var vm = (TeamSummaryViewModel)response.Model;
		Assert.Equal("30–39", vm.Team.Display);
	}

	[Fact]
	public async Task CanGetOverallResultsForTeam()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new TeamController(dataService);

		//act
		var response = await controller.Index(3);

		//assert
		var vm = (TeamSummaryViewModel)response.Model;
		Assert.Equal(1, vm.Overall.Rank.Value);
	}

	[Fact]
	public async Task CanGetCourseResultsForTeam()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new TeamController(dataService);

		//act
		var response = await controller.Index(3);

		//assert
		var vm = (TeamSummaryViewModel)response.Model;
		Assert.Equal(1, vm.Courses.First().Value.Rank.Value);
	}

	[Fact]
	public async Task CanGetTeamMembers()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(LeaderboardData.Courses);
		var controller = new TeamController(dataService);

		//act
		var response = await controller.Members(3);

		//assert
		var vm = (TeamMembersViewModel)response.Model;
		Assert.Equal(2, vm.RankedResults.Count);
		Assert.Equal(LeaderboardData.Athlete2, vm.RankedResults.First().Result.Athlete);
		Assert.Equal(LeaderboardData.Athlete4, vm.RankedResults.Skip(1).First().Result.Athlete);
	}
}