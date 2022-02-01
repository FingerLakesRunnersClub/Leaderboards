using System.Threading.Tasks;
using FLRC.Leaderboards.Challenge.Controllers;
using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Tests;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Challenge.Tests;

public class CompletedControllerTests
{
	[Fact]
	public async Task CanGetListOfAthletes()
	{
		//arrange
		var course = new Course
		{
			Results = CourseData.Results
		};
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns(new[] { course });

		var controller = new CompletedController(dataService);

		//act
		var response = await controller.Index();

		//assert
		var vm = (CompletedViewModel)response.Model;
		Assert.Equal(4, vm.RankedResults.Count);
	}
}