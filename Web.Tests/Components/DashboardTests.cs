using System.Security.Claims;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.Services;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class DashboardTests
{
    private static readonly ClaimsPrincipal User =
        new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")]));

    [Fact]
    public async Task EmptyWhenNotLoggedIn()
    {
        //arrange
        var authService = Substitute.For<IAuthService>();
        var athleteService = Substitute.For<IAthleteService>();
        var iterationManager = Substitute.For<IIterationManager>();

        var dashboard = new Dashboard(authService, athleteService, iterationManager);

        //act
        var result = await dashboard.InvokeAsync();

        //assert
        var content = result as ContentViewComponentResult;
        Assert.Empty(content!.Content);
    }

    [Fact]
    public async Task EmptyWhenNoAthleteForUser()
    {
        //arrange
        var authService = Substitute.For<IAuthService>();
        var athleteService = Substitute.For<IAthleteService>();
        var iterationManager = Substitute.For<IIterationManager>();

        var dashboard = new Dashboard(authService, athleteService, iterationManager);

        authService.IsLoggedIn().Returns(true);
        authService.GetCurrentUser().Returns(User);

        //act
        var result = await dashboard.InvokeAsync();

        //assert
        var content = result as ContentViewComponentResult;
        Assert.Empty(content!.Content);
    }

    [Fact]
    public async Task EmptyWhenNoActiveIteration()
    {
        //arrange
        var authService = Substitute.For<IAuthService>();
        var athleteService = Substitute.For<IAthleteService>();
        var iterationManager = Substitute.For<IIterationManager>();

        var dashboard = new Dashboard(authService, athleteService, iterationManager);

        authService.IsLoggedIn().Returns(true);
        authService.GetCurrentUser().Returns(User);

        athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(new Athlete());

        iterationManager.ActiveIteration().Returns(new Iteration { EndDate = new DateOnly(2025, 12, 31) });

        //act
        var result = await dashboard.InvokeAsync();

        //assert
        var content = result as ContentViewComponentResult;
        Assert.Empty(content!.Content);
    }

    [Fact]
    public async Task ShowsRegistrationPromptWhenNotRegisteredViaWebScorerYet()
    {
        //arrange
        var authService = Substitute.For<IAuthService>();
        var athleteService = Substitute.For<IAthleteService>();
        var iterationManager = Substitute.For<IIterationManager>();

        var dashboard = new Dashboard(authService, athleteService, iterationManager);

        authService.IsLoggedIn().Returns(true);
        authService.GetCurrentUser().Returns(User);

        athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(new Athlete());

        iterationManager.ActiveIteration().Returns(new Iteration { RegistrationType = nameof(WebScorer) });

        //act
        var result = await dashboard.InvokeAsync();

        //assert
        var content = result as ViewViewComponentResult;
        Assert.Equal("Register", content!.ViewName);
    }

    [Fact]
    public async Task ShowsSelectionPromptWhenNoChallengeSelected()
    {
        //arrange
        var authService = Substitute.For<IAuthService>();
        var athleteService = Substitute.For<IAthleteService>();
        var iterationManager = Substitute.For<IIterationManager>();

        var dashboard = new Dashboard(authService, athleteService, iterationManager);

        authService.IsLoggedIn().Returns(true);
        authService.GetCurrentUser().Returns(User);

        var iteration = new Iteration { RegistrationType = nameof(WebScorer) };
        iterationManager.ActiveIteration().Returns(iteration);

        var athlete = new Athlete { Registrations = [iteration] };
        athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

        //act
        var result = await dashboard.InvokeAsync();

        //assert
        var content = result as ViewViewComponentResult;
        Assert.Equal("Select", content!.ViewName);
    }

    [Fact]
    public async Task ShowsProgressWhenChallengeIsActive()
    {
        //arrange
        var authService = Substitute.For<IAuthService>();
        var athleteService = Substitute.For<IAthleteService>();
        var iterationManager = Substitute.For<IIterationManager>();

        var dashboard = new Dashboard(authService, athleteService, iterationManager);

        authService.IsLoggedIn().Returns(true);
        authService.GetCurrentUser().Returns(User);

        var iteration = new Iteration { RegistrationType = nameof(WebScorer) };
        iterationManager.ActiveIteration().Returns(iteration);

        var challenge = new Challenge { Iteration = iteration, IsPrimary = true };
        var athlete = new Athlete { Registrations = [iteration], Challenges = [challenge] };
        athleteService.Find(Arg.Any<string>(), Arg.Any<string>()).Returns(athlete);

        //act
        var result = await dashboard.InvokeAsync();

        //assert
        var content = result as ViewViewComponentResult;
        Assert.Equal("Progress", content!.ViewName);
    }
}