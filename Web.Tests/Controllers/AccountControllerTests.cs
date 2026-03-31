using System.Security.Principal;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Model;
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
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();

		var discourse = Substitute.For<IDiscourseAuthenticator>();
		discourse.GetLoginURL(Arg.Any<string>(), Arg.Any<string>()).Returns("https://example.com/login-page");

		var controller = new AccountController(adminService, athleteService, authService, discourse);

		//act
		var result = controller.Login();

		//assert
		Assert.IsType<RedirectResult>(result);
	}

	[Fact]
	public async Task RedirectPerformsLogin()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();
		var discourse = Substitute.For<IDiscourseAuthenticator>();

		var controller = new AccountController(adminService, athleteService, authService, discourse);

		discourse.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

		//act
		await controller.Redirect("test", "123");

		//assert
		await authService.Received().LogIn(Arg.Any<IIdentity>());
	}

	[Fact]
	public async Task RedirectGoesToWizardIfUserHasNoAthlete()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();
		var discourse = Substitute.For<IDiscourseAuthenticator>();

		var controller = new AccountController(adminService, athleteService, authService, discourse);

		discourse.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

		//act
		var result = await controller.Redirect("test", "123", "/test");

		//assert
		var redirect = result as RedirectResult;
		Assert.Equal("/Wizard", redirect!.Url);
	}

	[Fact]
	public async Task RedirectGoesToReturnURLIfSetForAthlete()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();
		var discourse = Substitute.For<IDiscourseAuthenticator>();

		var controller = new AccountController(adminService, athleteService, authService, discourse);

		var claims = new Dictionary<string, string> { { "external_id", "123"} };
		discourse.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
		discourse.ParseResponse(Arg.Any<string>()).Returns(claims);
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(new Athlete());

		//act
		var result = await controller.Redirect("test", "123", "/test");

		//assert
		var redirect = result as RedirectResult;
		Assert.Equal("/test", redirect!.Url);
	}

	[Fact]
	public async Task RedirectDoesNotAttemptLoginOnValidationFailure()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();

		var discourse = Substitute.For<IDiscourseAuthenticator>();
		discourse.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

		var controller = new AccountController(adminService, athleteService, authService, discourse);

		//act
		await controller.Redirect("test", "123");

		//assert
		await authService.DidNotReceive().LogIn(Arg.Any<IIdentity>());
	}

	[Fact]
	public async Task LogoutSignsOutUser()
	{
		//arrange
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var authService = Substitute.For<IAuthService>();
		var discourse = Substitute.For<IDiscourseAuthenticator>();
		var controller = new AccountController(adminService, athleteService, authService, discourse);

		//act
		await controller.Logout();

		//assert
		await authService.Received().LogOut();
	}
}