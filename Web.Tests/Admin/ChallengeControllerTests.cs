using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin.Controllers;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Admin;

public sealed class ChallengeControllerTests
{
	[Fact]
	public async Task CanShowAllChallenges()
	{
		//arrange
		var challengeService = Substitute.For<IChallengeService>();
		var courseService = Substitute.For<ICourseService>();
		var controller = new ChallengesController(challengeService, courseService);

		//act
		var result = await controller.Index();

		//assert
		Assert.Null(result.ViewName);
	}

	[Fact]
	public async Task CanShowEditForm()
	{
		//arrange
		var challengeService = Substitute.For<IChallengeService>();
		var courseService = Substitute.For<ICourseService>();
		var controller = new ChallengesController(challengeService, courseService);

		var id = Guid.NewGuid();
		challengeService.Get(id).Returns(new Challenge());

		//act
		var result = await controller.Edit(id);

		//assert
		Assert.Equal("Form", result.ViewName);
	}

	[Fact]
	public async Task CanSaveEditedChallenge()
	{
		//arrange
		var challengeService = Substitute.For<IChallengeService>();
		var courseService = Substitute.For<ICourseService>();
		var controller = new ChallengesController(challengeService, courseService);

		var id = Guid.NewGuid();
		challengeService.Get(id).Returns(new Challenge());

		//act
		var result = await controller.Edit(id, "test2", null, []);

		//assert
		Assert.Equal(nameof(ChallengesController.Index), result.ActionName);
	}
}