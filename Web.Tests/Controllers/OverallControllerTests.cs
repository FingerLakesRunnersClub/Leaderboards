using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
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
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var controller = new OverallController(iterationManager, overall, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var response = await controller.Points(Category.M.Display);

		//assert
		var vm = response.Model as ViewModel<OverallResults<Points>>;
		Assert.Equal(OverallData.Athlete1, vm!.Data.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetMostPointsForLimitedEvents()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var controller = new OverallController(iterationManager, overall, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var response = await controller.PointsTop3(Category.M.Display);

		//assert
		var vm = response.Model as ViewModel<OverallResults<Points>>;
		Assert.Equal(OverallData.Athlete1, vm!.Data.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetMostMiles()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var controller = new OverallController(iterationManager, overall, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var response = await controller.Miles();

		//assert
		var vm = response.Model as ViewModel<OverallResults<Miles>>;
		Assert.Equal(OverallData.Athlete2, vm!.Data.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetMostCourses()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var controller = new OverallController(iterationManager, overall, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var response = await controller.Courses();

		//assert
		var vm = response.Model as ViewModel<OverallResults<Count>>;
		Assert.Equal(OverallData.Athlete1, vm!.Data.RankedResults.First().Result.Athlete);
	}

	[Fact]
	public async Task CanGetBestAgeGradeAverage()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var overall = new OverallResultsCalculator(Substitute.For<ICommunityStarCalculator>());
		var controller = new OverallController(iterationManager, overall, TestHelpers.Config);

		iterationManager.ActiveIteration().Returns(OverallData.Iteration);

		//act
		var response = await controller.AgeGrade();

		//assert
		var vm = response.Model as ViewModel<OverallResults<AgeGrade>>;
		Assert.Equal(OverallData.Athlete1, vm!.Data.RankedResults.First().Result.Athlete);
	}
}