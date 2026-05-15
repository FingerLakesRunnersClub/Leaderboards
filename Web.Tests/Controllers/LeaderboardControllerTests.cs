using FLRC.Leaderboards.Core.Leaders;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;
using LeaderboardTable = FLRC.Leaderboards.Web.ViewModels.LeaderboardTable;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class LeaderboardControllerTests
{
	[Fact]
	public async Task ContainsOverallAndCourseData()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var calculator = Substitute.For<ILeaderboardCalculator>();
		var controller = new LeaderboardController(iterationManager, calculator);

		var leaderboard = new Leaderboard
		{
			OverallResults = [new LeaderboardTable()],
			OfficialCourses = new Dictionary<Course, LeaderboardTable[]> { { new Course(), [new LeaderboardTable()] } },
			OtherCourses = new Dictionary<Course, LeaderboardTable[]> { { new Course(), [new LeaderboardTable()] } }
		};
		calculator.GetLeaderboard(Arg.Any<Iteration>(), Arg.Any<LeaderboardResultType>(), Arg.Any<byte>()).Returns(leaderboard);

		//act
		var result = await controller.Index();

		//assert
		var vm = result.Model as ViewModel<Leaderboard>;
		Assert.NotEmpty(vm!.Data.OverallResults);
		Assert.NotEmpty(vm!.Data.OfficialCourses);
		Assert.NotEmpty(vm!.Data.OtherCourses);
	}
}