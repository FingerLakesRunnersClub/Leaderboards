using System.Security.Principal;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class AccountControllerTests
{
	[Fact]
	public void LoginRedirectsToAuthPage()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();

		var discourse = Substitute.For<IDiscourseAuthenticator>();
		discourse.GetLoginURL(Arg.Any<string>()).Returns("https://example.com/login-page");

		var controller = new AccountController(athleteService, authService, discourse);

		//act
		var result = controller.Login();

		//assert
		Assert.IsType<RedirectResult>(result);
	}

	[Fact]
	public async Task RedirectPerformsLogin()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();
		var discourse = Substitute.For<IDiscourseAuthenticator>();

		var controller = new AccountController(athleteService, authService, discourse);

		discourse.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

		//act
		await controller.Redirect("test", "123");

		//assert
		await authService.Received().LogIn(Arg.Any<IIdentity>());
	}

	[Fact]
	public async Task RedirectDoesNotAttemptLoginOnValidationFailure()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();

		var discourse = Substitute.For<IDiscourseAuthenticator>();
		discourse.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

		var controller = new AccountController(athleteService, authService, discourse);

		//act
		await controller.Redirect("test", "123");

		//assert
		await authService.DidNotReceive().LogIn(Arg.Any<IIdentity>());
	}

	[Fact]
	public async Task LogoutSignsOutUser()
	{
		//arrange
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();
		var discourse = Substitute.For<IDiscourseAuthenticator>();
		var controller = new AccountController(athleteService, authService, discourse);

		//act
		await controller.Logout();

		//assert
		await authService.Received().LogOut();
	}
}