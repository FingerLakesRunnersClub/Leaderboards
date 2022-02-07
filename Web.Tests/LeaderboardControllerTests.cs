using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web;

public class LeaderboardControllerTests
{
	[Fact]
	public async Task CanGetAllCourses()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var controller = new LeaderboardController(dataService, TestHelpers.Config);

		//act
		await controller.Index();

		//assert
		await dataService.Received().GetAllResults();
	}
}