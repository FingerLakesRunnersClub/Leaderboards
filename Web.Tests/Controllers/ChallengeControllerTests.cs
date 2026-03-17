using System.Security.Claims;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Controllers;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Controllers;

public sealed class ChallengeControllerTests
{
	[Fact]
	public async Task CanRenderDashboard()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
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
		var result = await controller.Dashboard();

		//assert
		var view = result as ViewResult;
		Assert.Null(view!.ViewName);
	}

	[Fact]
	public async Task RegistrationShowsRefreshFormIfNotRegistered()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
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
	public async Task RegistrationRedirectsToSelectionPageIfRegistered()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
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
		var redirect = result as RedirectToActionResult;
		Assert.Equal(nameof(ChallengeController.Select), redirect!.ActionName);
	}

	[Fact]
	public async Task CanUpdateRegistration()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		iterationManager.ActiveIteration().Returns(new Iteration());

		//act
		await controller.Registration(new FormCollection([]));

		//assert
		await registrationManager.Received().Update(Arg.Any<Iteration>());
	}

	[Fact]
	public async Task CanShowSelectionForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Registrations = [iteration] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Select();

		//assert
		var view = result as ViewResult;
		Assert.Null(view!.ViewName);
	}

	[Fact]
	public async Task SelectionFormRedirectsToRegistrationWhenNotRegistered()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Select();

		//assert
		var view = result as RedirectToActionResult;
		Assert.Equal(nameof(ChallengeController.Registration), view!.ActionName);
	}

	[Fact]
	public async Task CanShowConfirmationForm()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Registrations = [iteration] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Select(new SelectChallengeForm());

		//assert
		var view = result as ViewResult;
		Assert.Equal("Confirm", view!.ViewName);
	}

	[Fact]
	public async Task ConfirmationFormRedirectsToRegistrationWhenNotRegistered()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		var result = await controller.Select(new SelectChallengeForm());

		//assert
		var view = result as RedirectToActionResult;
		Assert.Equal(nameof(ChallengeController.Registration), view!.ActionName);
	}

	[Fact]
	public async Task SubmittingOfficialChallengeConfirmationFormSavesData()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Registrations = [iteration] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		await controller.Confirm(new SelectChallengeForm { Selection = "Official" });

		//assert
		await challengeService.DidNotReceive().Add(Arg.Any<Challenge>());
		await challengeService.Received().AddConnection(athlete, iteration.OfficialChallenge!);
	}

	[Fact]
	public async Task SubmittingPersonalChallengeConfirmationFormSavesData()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(authService, athleteService, challengeService, iterationManager, registrationManager);

		var claims = new[] { new Claim("external_id", "123"), new Claim("email", "test@example.com") };
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity(claims)));

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Registrations = [iteration] };
		athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

		//act
		await controller.Confirm(new SelectChallengeForm { Selection = "Personal" });

		//assert
		await challengeService.Received().Add(Arg.Any<Challenge>());
		await challengeService.Received().AddConnection(athlete, Arg.Any<Challenge>());
	}
}