using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class GroupControllerTests
{
	[Fact]
	public async Task CanGetGroupMembers()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetGroupMembers("Test").Returns([CourseData.Athlete1, CourseData.Athlete3]);
		dataService.GetAllResults().Returns([new Course { Results = CourseData.Results, Distance = new Distance("10K")}]);
		var controller = new GroupController(dataService, TestHelpers.Config);

		//act
		var result = await controller.Index("Test");

		//assert
		var vm = (GroupResultsViewModel)result.Model;
		Assert.Equal(2, vm!.RankedResults.Count);
	}
}