using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Web.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class AuthControllerTests
{
	private static ControllerContext DefaultContext() => new() { HttpContext = new DefaultHttpContext { Request = { Scheme = "https", Host = new HostString("localhost") } } };

	[Fact]
	public void LoginRedirectsToAuthPage()
	{
		//arrange
		var auth = Substitute.For<IDiscourseAuthenticator>();
		auth.GetLoginURL(Arg.Any<string>()).Returns("https://example.com/login-page");

		var context = Substitute.For<IHttpContextAccessor>();
		var controller = new AuthController(auth, context) { ControllerContext = DefaultContext() };

		//act
		var result = controller.Login();

		//assert
		Assert.IsType<RedirectResult>(result);
	}

	[Fact]
	public void InfoProvidesUserDetails()
	{
		//arrange
		var auth = Substitute.For<IDiscourseAuthenticator>();
		auth.GetLoginURL(Arg.Any<string>()).Returns("https://example.com/login-page");

		var context = Substitute.For<IHttpContextAccessor>();
		context.HttpContext = new DefaultHttpContext { User = new GenericPrincipal(new ClaimsIdentity([new Claim("name", "Steve Desmond")]), []) };

		var controller = new AuthController(auth, context) { ControllerContext = DefaultContext() };

		//act
		var result = controller.Info();

		//assert
		var content = result as ContentResult;
		var json = JsonDocument.Parse(content!.Content!);
		Assert.Equal("Steve Desmond", json.RootElement.GetProperty("name").GetString());
	}

	[Fact]
	public void InfoIsEmptyWhenNotLoggedIn()
	{
		//arrange
		var auth = Substitute.For<IDiscourseAuthenticator>();
		var context = Substitute.For<IHttpContextAccessor>();
		var controller = new AuthController(auth, context) { ControllerContext = DefaultContext() };

		//act
		var result = controller.Info();

		//assert
		var content = result as ContentResult;
		Assert.Equal("{}", content!.Content);
	}

	[Fact]
	public async Task RedirectPerformsLogin()
	{
		//arrange
		var auth = Substitute.For<IDiscourseAuthenticator>();
		auth.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

		var context = Substitute.For<IHttpContextAccessor>();
		var authService = Substitute.For<IAuthenticationService>();
		context.HttpContext!.RequestServices.GetService(typeof(IAuthenticationService)).Returns(authService);
		context.HttpContext!.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(Substitute.For<IUrlHelperFactory>());

		var controller = new AuthController(auth, context) { ControllerContext = DefaultContext() };

		//act
		await controller.Redirect("test", "123");

		//assert
		await authService.Received().SignInAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<ClaimsPrincipal>(), Arg.Any<AuthenticationProperties>());
	}

	[Fact]
	public async Task RedirectDoesNotAttemptLoginOnValidationFailure()
	{
		//arrange
		var auth = Substitute.For<IDiscourseAuthenticator>();
		auth.IsValidResponse(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

		var context = Substitute.For<IHttpContextAccessor>();
		var authService = Substitute.For<IAuthenticationService>();
		context.HttpContext!.RequestServices.GetService(typeof(IAuthenticationService)).Returns(authService);
		context.HttpContext!.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(Substitute.For<IUrlHelperFactory>());

		var controller = new AuthController(auth, context) { ControllerContext = DefaultContext() };

		//act
		await controller.Redirect("test", "123");

		//assert
		await authService.DidNotReceive().SignInAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<ClaimsPrincipal>(), Arg.Any<AuthenticationProperties>());
	}

	[Fact]
	public async Task LogoutSignsOutUser()
	{
		//arrange
		var auth = Substitute.For<IDiscourseAuthenticator>();

		var context = Substitute.For<IHttpContextAccessor>();
		var authService = Substitute.For<IAuthenticationService>();
		context.HttpContext!.RequestServices.GetService(typeof(IAuthenticationService)).Returns(authService);
		context.HttpContext!.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(Substitute.For<IUrlHelperFactory>());

		var controller = new AuthController(auth, context) { ControllerContext = DefaultContext() };

		//act
		await controller.Logout();

		//assert
		await authService.Received().SignOutAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<AuthenticationProperties>());
	}
}