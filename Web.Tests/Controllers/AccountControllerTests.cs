using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Http;
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
		var authService = Substitute.For<IAuthService>();

		var discourse = Substitute.For<IDiscourseAuthenticator>();
		discourse.GetLoginURL(Arg.Any<string>()).Returns("https://example.com/login-page");

		var contextAccessor = Substitute.For<IHttpContextAccessor>();

		var controller = new AccountController(authService, discourse, contextAccessor);

		//act
		var result = controller.Login();

		//assert
		Assert.IsType<RedirectResult>(result);
	}

	[Fact]
	public void InfoProvidesUserDetails()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", "Steve Desmond")])));

		var discourse = Substitute.For<IDiscourseAuthenticator>();
		discourse.GetLoginURL(Arg.Any<string>()).Returns("https://example.com/login-page");

		var contextAccessor = Substitute.For<IHttpContextAccessor>();

		var controller = new AccountController(authService, discourse, contextAccessor);

		//act
		var result = controller.Info();

		//assert
		var json = JsonDocument.Parse(result!.Content!);
		Assert.Equal("Steve Desmond", json.RootElement.GetProperty("name").GetString());
	}

	[Fact]
	public void InfoIsEmptyWhenNotLoggedIn()
	{
		//arrange
		var discourse = Substitute.For<IDiscourseAuthenticator>();
		var authService = Substitute.For<IAuthService>();
		var contextAccessor = Substitute.For<IHttpContextAccessor>();

		var controller = new AccountController(authService, discourse, contextAccessor);

		//act
		var result = controller.Info();

		//assert
		Assert.Equal("{}", result!.Content);
	}

	[Fact]
	public async Task RedirectPerformsLogin()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();

		var discourse = Substitute.For<IDiscourseAuthenticator>();
		discourse.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

		var contextAccessor = Substitute.For<IHttpContextAccessor>();

		var controller = new AccountController(authService, discourse, contextAccessor);

		//act
		await controller.Redirect("test", "123");

		//assert
		await authService.Received().LogIn(Arg.Any<IIdentity>());
	}

	[Fact]
	public async Task RedirectDoesNotAttemptLoginOnValidationFailure()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();

		var discourse = Substitute.For<IDiscourseAuthenticator>();
		discourse.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

		var contextAccessor = Substitute.For<IHttpContextAccessor>();

		var controller = new AccountController(authService, discourse, contextAccessor);

		//act
		await controller.Redirect("test", "123");

		//assert
		await authService.DidNotReceive().LogIn(Arg.Any<IIdentity>());
	}

	[Fact]
	public async Task LogoutSignsOutUser()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var discourse = Substitute.For<IDiscourseAuthenticator>();
		var contextAccessor = Substitute.For<IHttpContextAccessor>();

		var controller = new AccountController(authService, discourse, contextAccessor);

		//act
		await controller.Logout();

		//assert
		await authService.Received().LogOut();
	}
}