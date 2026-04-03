using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class AthletesControllerTests
{
	[Fact]
	public async Task CanGetListOfAthletes()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new AthletesController(iterationManager);

		var athletes = new[]
		{
			new Athlete { Name = "Test 1" },
			new Athlete { Name = "Test 2" }
		};
		iterationManager.ActiveIteration().Returns(new Iteration { Athletes = athletes });

		//act
		var response = await controller.Index();

		//assert
		var vm = response.Model as ViewModel<Athlete[]>;
		Assert.Equal("Test 1", vm!.Data[0].Name);
		Assert.Equal("Test 2", vm.Data[1].Name);
	}
}