using System.Security.Claims;
using FLRC.Leaderboards.Core.Auth;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Areas.Admin;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
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
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		//act
		var result = await controller.Index();

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Link), redirect!.ActionName);
	}

	[Fact]
	public async Task IndexRedirectsToRegistrationCheckWhenMatchFound()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Index();

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Registration), redirect!.ActionName);
	}

	[Fact]
	public async Task LinkRedirectsToRegistrationCheckWhenMatchFound()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Link();

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Registration), redirect!.ActionName);
	}

	[Fact]
	public async Task CanShowLinkForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
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
	public async Task LinkSubmitRedirectsToRegistrationCheckWhenMatchFound()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Link(new FormCollection([]));

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Registration), redirect!.ActionName);
	}

	[Fact]
	public async Task LinkSubmitShowsErrorWhenLoginFails()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
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
	public async Task LinkSubmitRedirectsToRegistrationOnSuccess()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		legacyDataConverter.GetAthlete(nameof(WebScorer), Arg.Any<Core.Athletes.Athlete>()).Returns(new Athlete());

		//act
		var result = await controller.Link(new FormCollection([]));

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Registration), redirect!.ActionName);
	}

	[Fact]
	public async Task RegistrationRedirectsToLinkIfNoLinkedAccount()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(new Athlete());

		//act
		var result = await controller.Registration();

		//assert
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(WizardController.Link), redirect!.ActionName);
	}

	[Fact]
	public async Task RegistrationShowsRefreshFormIfNotRegistered()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		iterationManager.ActiveIteration().Returns(new Iteration());

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Registration();

		//assert
		var view = result as ViewResult;
		Assert.Null(view!.ViewName);
	}

	[Fact]
	public async Task RegistrationShowsCompletePageIfRegistered()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var iteration = new Iteration();
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete
		{
			LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }],
			Registrations = [iteration]
		};
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Registration();

		//assert
		var view = result as ViewResult;
		Assert.Equal("Complete", view!.ViewName);
	}

	[Fact]
	public async Task CanUpdateRegistration()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var legacyDataConverter = Substitute.For<ILegacyDataConverter>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var webScorer = Substitute.For<IWebScorerAuthenticator>();
		var controller = new WizardController(athleteService, authService, iterationManager, legacyDataConverter, registrationManager, webScorer);

		var claims = new[]
		{
			new Claim("external_id", "123"),
			new Claim("email", "test@example.com")
		};
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		iterationManager.ActiveIteration().Returns(new Iteration());

		//act
		await controller.Registration(new FormCollection([]));

		//assert
		await registrationManager.Received().Update(Arg.Any<Iteration>());
	}
}