using System.Security.Claims;
using System.Security.Principal;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Services;

public sealed class AuthServiceTests
{
	[Fact]
	public void CanGetCurrentHost()
	{
		//arrange
		var contextAccessor = Substitute.For<IHttpContextAccessor>();
		contextAccessor.HttpContext!.Request.Scheme = "https";
		contextAccessor.HttpContext!.Request.Host = new HostString("example.com");

		var service = new AuthService(contextAccessor);

		//act
		var host = service.GetCurrentHost();

		//assert
		Assert.Equal("https://example.com", host);
	}

	[Fact]
	public async Task CanLogInUser()
	{
		//arrange
		var contextAccessor = Substitute.For<IHttpContextAccessor>();
		var netAuthService = Substitute.For<IAuthenticationService>();
		contextAccessor.HttpContext!.RequestServices.GetService(typeof(IAuthenticationService)).Returns(netAuthService);
		contextAccessor.HttpContext!.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(Substitute.For<IUrlHelperFactory>());

		var service = new AuthService(contextAccessor);

		//act
		var identity = new GenericIdentity("test");
		await service.LogIn(identity);

		//assert
		await netAuthService.Received().SignInAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<ClaimsPrincipal>(), Arg.Any<AuthenticationProperties>());
	}

	[Fact]
	public void CanGetCurrentUser()
	{
		//arrange
		var contextAccessor = Substitute.For<IHttpContextAccessor>();
		contextAccessor.HttpContext!.User.Identity.Returns(new GenericIdentity("test"));

		var service = new AuthService(contextAccessor);

		//act
		var user = service.GetCurrentUser();

		//assert
		Assert.Equal("test", user.Identity!.Name);
	}

	[Fact]
	public void IsLoggedInWhenUserIsAuthenticated()
	{
		//arrange
		var contextAccessor = Substitute.For<IHttpContextAccessor>();
		contextAccessor.HttpContext!.User.Identity!.IsAuthenticated.Returns(true);

		var service = new AuthService(contextAccessor);

		//act
		var isLoggedIn = service.IsLoggedIn();

		//assert
		Assert.True(isLoggedIn);
	}

	[Fact]
	public void IsNotLoggedInWhenUserIsNotAuthenticated()
	{
		//arrange
		var contextAccessor = Substitute.For<IHttpContextAccessor>();
		contextAccessor.HttpContext!.User.Identity!.IsAuthenticated.Returns(false);

		var service = new AuthService(contextAccessor);

		//act
		var isLoggedIn = service.IsLoggedIn();

		//assert
		Assert.False(isLoggedIn);
	}

	[Fact]
	public void IsNotLoggedInWhenNoIdentity()
	{
		//arrange
		var contextAccessor = Substitute.For<IHttpContextAccessor>();
		contextAccessor.HttpContext!.User.Identity.ReturnsNull();

		var service = new AuthService(contextAccessor);

		//act
		var isLoggedIn = service.IsLoggedIn();

		//assert
		Assert.False(isLoggedIn);
	}

	[Fact]
	public async Task CanLogOutUser()
	{
		//arrange
		var contextAccessor = Substitute.For<IHttpContextAccessor>();
		var netAuthService = Substitute.For<IAuthenticationService>();
		contextAccessor.HttpContext!.RequestServices.GetService(typeof(IAuthenticationService)).Returns(netAuthService);
		contextAccessor.HttpContext!.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(Substitute.For<IUrlHelperFactory>());

		var service = new AuthService(contextAccessor);

		//act
		await service.LogOut();

		//assert
		await netAuthService.Received().SignOutAsync(Arg.Any<HttpContext>(), Arg.Any<string>(), Arg.Any<AuthenticationProperties>());
	}
}