using System.Threading.Tasks;
using FLRC.Leaderboards.Challenge.Controllers;
using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Tests;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Challenge.Tests;

public class GroupControllerTests
{
	[Fact]
	public async Task CanGetGroupMembers()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		dataService.GetGroupMembers("Test").Returns(new[]
		{
				CourseData.Athlete1,
				CourseData.Athlete3
			});
		dataService.GetAllResults().Returns(new[] { new Course { Results = CourseData.Results } });
		var controller = new GroupController(dataService);

		//act
		var result = await controller.Index("Test");

		//assert
		var vm = (GroupResultsViewModel)result.Model;
		Assert.Equal(2, vm.RankedResults.Count);
	}
}