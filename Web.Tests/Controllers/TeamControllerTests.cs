using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class TeamControllerTests
{
	[Fact]
	public async Task CanGetTeamNameFromViewModel()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new TeamController(iterationManager);

		//act
		var response = await controller.Index(3);

		//assert
		var vm = (TeamSummaryViewModel)response.Model;
		Assert.Equal("30–39", vm!.Team.Display);
	}

	[Fact]
	public async Task CanGetOverallResultsForTeam()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new TeamController(iterationManager);

		iterationManager.ActiveIteration().Returns(UltraChallengeData.Iteration);

		//act
		var response = await controller.Index(3);

		//assert
		var vm = (TeamSummaryViewModel)response.Model;
		Assert.Equal(1, vm!.Overall.Rank.Value);
	}

	[Fact]
	public async Task CanGetCourseResultsForTeam()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new TeamController(iterationManager);

		iterationManager.ActiveIteration().Returns(UltraChallengeData.Iteration);

		//act
		var response = await controller.Index(3);

		//assert
		var vm = (TeamSummaryViewModel)response.Model;
		Assert.Equal(1, vm!.Courses.First().Value.Rank.Value);
	}

	[Fact]
	public async Task CanGetTeamMembers()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new TeamController(iterationManager);

		iterationManager.ActiveIteration().Returns(UltraChallengeData.Iteration);

		//act
		var response = await controller.Members(3);

		//assert
		var vm = response.Model as ViewModel<TeamMembers>;
		Assert.Equal(2, vm!.Data.Results.Count);
		Assert.Equal(ResultsData.Athlete2, vm.Data.Results.First().Result.Athlete);
		Assert.Equal(ResultsData.Athlete4, vm.Data.Results.Skip(1).First().Result.Athlete);
	}
}