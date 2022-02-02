using FLRC.Leaderboards.Core;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web;

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