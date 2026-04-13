using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Teams;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class OverallControllerTests
{
	[Fact]
	public async Task CanGetMostPoints()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new OverallController(iterationManager, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(UltraChallengeData.Iteration);

		//act
		var response = await controller.Points(Category.M.Display);

		//assert
		var vm = response.Model as ViewModel<OverallResults<Points>>;
		Assert.Equal(ResultsData.Athlete1, vm!.Data.Results.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetMostPointsForLimitedEvents()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new OverallController(iterationManager, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(UltraChallengeData.Iteration);

		//act
		var response = await controller.PointsTop3(Category.M.Display);

		//assert
		var vm = response.Model as ViewModel<OverallResults<Points>>;
		Assert.Equal(ResultsData.Athlete1, vm!.Data.Results.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetMostMiles()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new OverallController(iterationManager, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(UltraChallengeData.Iteration);

		//act
		var response = await controller.Miles();

		//assert
		var vm = response.Model as ViewModel<OverallResults<Miles>>;
		Assert.Equal(ResultsData.Athlete2, vm!.Data.Results.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetBestAgeGradeAverage()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new OverallController(iterationManager, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(UltraChallengeData.Iteration);

		//act
		var response = await controller.AgeGrade();

		//assert
		var vm = response.Model as ViewModel<OverallResults<AgeGrade>>;
		Assert.Equal(ResultsData.Athlete1, vm!.Data.Results.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetTeamPoints()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new OverallController(iterationManager, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(UltraChallengeData.Iteration);

		//act
		var response = await controller.Team();

		//assert
		var vm = response.Model as ViewModel<OverallResults<TeamResults>>;
		Assert.Equal("1–29", vm!.Data.Results.First().Value.Team.Display);
	}
}