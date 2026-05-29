using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class UltraChallengeControllerTests
{
	[Fact]
	public async Task SelectsCorrectChallenge()
	{
		//arrange
		var iterationManager = Substitute.For<IIterationManager>();
		var controller = new UltraChallengesController(iterationManager);

		iterationManager.ActiveIteration().Returns(UltraChallengeData.Iteration);

		//act
		var result = await controller.Results(UltraChallengeData.OneHundredK.ID);

		//assert
		var vm = result.Model as ViewModel<ChallengeData>;
		Assert.Equal("Test 1", vm!.Data.Challenge.Name);
	}
}