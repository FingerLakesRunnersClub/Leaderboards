using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Reports;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class CompletedControllerTests
{
	[Fact]
	public async Task CanGetListOfCompletions()
	{
		//arrange
		var personal = new Dictionary<Athlete, DateOnly>
		{
			{ CourseData.Athlete1, new DateOnly(2023, 08, 09) },
			{ CourseData.Athlete2, new DateOnly(2023, 08, 09) }
		};
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };
		var dataService = Substitute.For<IDataService>();
		dataService.GetAllResults().Returns([course]);
		dataService.GetPersonalCompletions().Returns(personal);

		var controller = new CompletedController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Index();

		//assert
		var vm = (CompletedViewModel)response.Model;
		Assert.Equal(4, vm!.RankedResults.Count);
		Assert.Equal(2, vm!.PersonalResults.Count);
	}
}