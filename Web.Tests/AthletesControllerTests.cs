using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Tests;
using FLRC.Leaderboards.Web.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class AthletesControllerTests
{
	[Fact]
	public async Task CanGetListOfAthletes()
	{
		//arrange
		var athletes = new Dictionary<uint, Athlete>
		{
			{ 123, new Athlete { Name = "Test 1" } },
			{ 234, new Athlete { Name = "Test 2" } }
		};
		var dataService = Substitute.For<IDataService>();
		dataService.GetAthletes().Returns(athletes);

		var controller = new AthletesController(dataService, TestHelpers.Config);

		//act
		var response = await controller.Index();

		//assert
		var vm = response.Model as AthletesViewModel;
		Assert.Equal("Test 1", vm!.Athletes[123].Name);
		Assert.Equal("Test 2", vm.Athletes[234].Name);
	}
}