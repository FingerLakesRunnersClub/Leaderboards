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
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration();
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete
		{
			LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }],
			Registrations = [iteration],
			Challenges = [new Challenge { Iteration = iteration, IsPrimary = true }]
		};
		sessionManager.GetCurrentAthlete().Returns(athlete);

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
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		iterationManager.ActiveIteration().Returns(new Iteration());

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		sessionManager.GetCurrentAthlete().Returns(athlete);

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
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration();
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete
		{
			LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }],
			Registrations = [iteration]
		};
		sessionManager.GetCurrentAthlete().Returns(athlete);

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
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

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
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Registrations = [iteration] };
		sessionManager.GetCurrentAthlete().Returns(athlete);

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
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		sessionManager.GetCurrentAthlete().Returns(athlete);

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
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Registrations = [iteration] };
		sessionManager.GetCurrentAthlete().Returns(athlete);

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
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		sessionManager.GetCurrentAthlete().Returns(athlete);

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
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Registrations = [iteration] };
		sessionManager.GetCurrentAthlete().Returns(athlete);

		//act
		await controller.Confirm(new SelectChallengeForm { Selection = Challenge.Types.Classic });

		//assert
		await challengeService.DidNotReceive().Add(Arg.Any<Challenge>());
		await challengeService.Received().AddConnection(athlete, iteration.OfficialChallenge!);
	}

	[Fact]
	public async Task SubmittingPersonalChallengeConfirmationFormSavesData()
	{
		//arrange
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Registrations = [iteration] };
		sessionManager.GetCurrentAthlete().Returns(athlete);

		//act
		await controller.Confirm(new SelectChallengeForm { Selection = Challenge.Types.Personal });

		//assert
		await challengeService.Received().Add(Arg.Any<Challenge>());
		await challengeService.Received().AddConnection(athlete, Arg.Is<Challenge>(c => c.IsPrimary && !c.IsOfficial));
	}

	[Fact]
	public async Task CanViewDeleteConfirmationForm()
	{
		//arrange
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Registrations = [iteration] };
		sessionManager.GetCurrentAthlete().Returns(athlete);

		//act
		var result = await controller.Delete();

		//assert
		Assert.NotNull(result.Model);
	}

	[Fact]
	public async Task DeleteConfirmationFormRedirectsToDashboardWhenNoIterationFound()
	{
		//arrange
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		//act
		var result = await controller.Delete(new FormCollection([]));

		//assert
		Assert.Equal(nameof(ChallengeController.Dashboard), result.ActionName);
	}

	[Fact]
	public async Task DeleteConfirmationFormRedirectsToRegistrationWhenNotRegistered()
	{
		//arrange
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var iteration = new Iteration { Challenges = [new Challenge { IsOfficial = true, IsPrimary = true }] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }] };
		sessionManager.GetCurrentAthlete().Returns(athlete);

		//act
		var result = await controller.Delete(new FormCollection([]));

		//assert
		Assert.Equal(nameof(ChallengeController.Registration), result.ActionName);
	}

	[Fact]
	public async Task DeleteConfirmationRemovesConnectionWhenChallengeExists()
	{
		//arrange
		var sessionManager = Substitute.For<ISessionManager>();
		var challengeService = Substitute.For<IChallengeService>();
		var iterationManager = Substitute.For<IIterationManager>();
		var registrationManager = Substitute.For<IRegistrationManager>();
		var controller = new ChallengeController(sessionManager, challengeService, iterationManager, registrationManager);

		var challenge = new Challenge { IsOfficial = true, IsPrimary = true };
		var iteration = new Iteration { Challenges = [challenge] };
		iterationManager.ActiveIteration().Returns(iteration);

		var athlete = new Athlete { LinkedAccounts = [new LinkedAccount { Type = LinkedAccount.Keys.WebScorer, Value = "234" }], Challenges = [challenge]};
		sessionManager.GetCurrentAthlete().Returns(athlete);

		//act
		await controller.Delete(new FormCollection([]));

		//assert
		await challengeService.RemoveConnection(athlete, challenge);
	}
}