using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
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
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var controller = new TeamController(iterationManager, overall);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var response = await controller.Index(3);

		//assert
		var vm = response.Model as ViewModel<TeamSummary>;
		Assert.Equal("30–39", vm!.Data.Team.Display);
	}

	[Fact]
	public async Task CanGetOverallResultsForTeam()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var controller = new TeamController(iterationManager, overall);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var response = await controller.Index(3);

		//assert
		var vm = response.Model as ViewModel<TeamSummary>;
		Assert.Equal(1, vm!.Data.Overall.Rank.Value);
	}

	[Fact]
	public async Task CanGetCourseResultsForTeam()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var controller = new TeamController(iterationManager, overall);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var response = await controller.Index(3);

		//assert
		var vm = response.Model as ViewModel<TeamSummary>;
		Assert.Equal(1, vm!.Data.Courses.First().Value.Rank.Value);
	}

	[Fact]
	public async Task CanGetTeamMembers()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var controller = new TeamController(iterationManager, overall);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var response = await controller.Members(3);

		//assert
		var vm = response.Model as ViewModel<TeamMembers>;
		Assert.Equal(2, vm!.Data.RankedResults.Count);
		Assert.Equal(OverallData.Athlete2, vm.Data.RankedResults.First().Result.Athlete);
		Assert.Equal(OverallData.Athlete4, vm.Data.RankedResults.Skip(1).First().Result.Athlete);
	}
}