using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class LeaderboardControllerTests
{
	[Fact]
	public async Task ContainsOverallAndCourseData()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new LeaderboardController(iterationManager, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var result = await controller.Index();

		//assert
		var vm = result.Model as ViewModel<Leaderboard>;
		Assert.NotEmpty(vm!.Data.OverallResults);
		Assert.NotEmpty(vm!.Data.OtherCourses);
	}
}