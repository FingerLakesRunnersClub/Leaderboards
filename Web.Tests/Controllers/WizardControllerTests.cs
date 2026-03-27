using System.Security.Claims;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class WizardControllerTests
{
	[Fact]
	public async Task IndexRedirectsToLinkWhenMatchNotFound()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, legacyDataConverter, webScorer);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		//act
		var result = await controller.Index();

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Link), redirect!.ActionName);
	}

	[Fact]
	public async Task IndexRedirectsToCompleteWhenMatchFound()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, legacyDataConverter, webScorer);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var athlete = new Athlete
		{
			LinkedAccounts =
			[
				new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "234" },
				new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "345" }
			]
		};
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Index();

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Complete), redirect!.ActionName);
	}

	[Fact]
	public async Task LinkRedirectsToCompleteWhenMatchIsComplete()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, legacyDataConverter, webScorer);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var athlete = new Athlete
		{
			LinkedAccounts =
			[
				new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "234" },
				new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "345" }
			]
		};
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Link();

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Complete), redirect!.ActionName);
	}

	[Fact]
	public async Task CanShowLinkForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, legacyDataConverter, webScorer);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var athlete = new Athlete();
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Link();

		//assert
		var view = result as ViewResult;
		Assert.IsType<ViewModel<string>>(view!.Model);
	}

	[Fact]
	public async Task LinkSubmitRedirectsToCompleteWhenMatchFound()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, legacyDataConverter, webScorer);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var athlete = new Athlete
		{
			LinkedAccounts =
			[
				new LinkedAccount { Type = LinkedAccount.Keys.Discourse, Value = "234" },
				new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "345" }
			]
		};
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Link(new FormCollection([]));

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Complete), redirect!.ActionName);
	}

	[Fact]
	public async Task LinkSubmitShowsErrorWhenLoginFails()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, legacyDataConverter, webScorer);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		webScorer.Login(Arg.Any<string>(), Arg.Any<string>()).Throws(new HttpRequestException());

		//act
		var result = await controller.Link(new FormCollection([]));

		//assert
		var view = result as ViewResult;
		var vm = view!.Model as ViewModel<string>;
		Assert.Contains("verification failed", vm!.Data);
	}

	[Fact]
	public async Task LinkSubmitRedirectsToCompleteOnSuccess()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, legacyDataConverter, webScorer);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		legacyDataConverter.GetAthlete(nameof(WebScorer), Arg.Any<Core.Athletes.Athlete>()).Returns(new Athlete());

		//act
		var result = await controller.Link(new FormCollection([]));

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Complete), redirect!.ActionName);
	}
}