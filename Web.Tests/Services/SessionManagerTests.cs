using System.Security.Claims;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Services;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Services;

public class SessionManagerTests
{
	[Fact]
	public async Task CanGetCurrentAthlete()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();

		var sessionManager = new SessionManager(authService, athleteService);

		authService.IsLoggedIn().Returns(true);
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123")])));

		athleteService.Find(LinkedAccount.Keys.Discourse, "123").Returns(new Athlete { Name = "Test" });

		//act
		var athlete = await sessionManager.GetCurrentAthlete();

		//assert
		Assert.Equal("Test", athlete.Name);
	}

	[Fact]
	public async Task CanGetCurrentAthleteByEmailAsBackup()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();

		var sessionManager = new SessionManager(authService, athleteService);

		authService.IsLoggedIn().Returns(true);
		authService.GetCurrentUser().Returns(new ClaimsPrincipal(new ClaimsIdentity([new Claim("external_id", "123"), new Claim("email", "test@example.com")])));

		athleteService.Find(LinkedAccount.Keys.Email, "test@example.com").Returns(new Athlete { Name = "Test" });

		//act
		var athlete = await sessionManager.GetCurrentAthlete();

		//assert
		Assert.Equal("Test", athlete.Name);
	}

	[Fact]
	public async Task DoesNotCheckWhenNotLoggedIn()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var athleteService = Substitute.For<IAthleteService>();

		var sessionManager = new SessionManager(authService, athleteService);

		authService.IsLoggedIn().Returns(false);

		//act
		var athlete = await sessionManager.GetCurrentAthlete();

		//assert
		Assert.Null(athlete);
		await athleteService.DidNotReceive().Find(Arg.Any<string>(), Arg.Any<string>());
	}
}