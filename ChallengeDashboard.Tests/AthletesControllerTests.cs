using System.Collections.Generic;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class AthletesControllerTests
{
	[Fact]
	public async Task CanGetListOfAthletes()
	{
		//arrange
		var athletes = new Dictionary<uint, Athlete>
			{
				{123, new Athlete {Name = "Test 1"}},
				{234, new Athlete {Name = "Test 2"}}
			};
		var dataService = Substitute.For<IDataService>();
		dataService.GetAthletes().Returns(athletes);

		var controller = new AthletesController(dataService);

		//act
		var response = await controller.Index();

		//assert
		var vm = response.Model as AthletesViewModel;
		Assert.Equal("Test 1", vm.Athletes[123].Name);
		Assert.Equal("Test 2", vm.Athletes[234].Name);
	}
}
