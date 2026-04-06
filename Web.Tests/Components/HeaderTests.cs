using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.Services;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class HeaderTests
{
	[Fact]
	public async Task CanRenderComponent()
	{
		//arrange
		var authService = Substitute.For<IAuthService>();
		var adminService = Substitute.For<IAdminService>();
		var athleteService = Substitute.For<IAthleteService>();
		var iterationManager = Substitute.For<IIterationManager>();

		var settings = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var config = new AppConfig(settings);

		var component = new Header(authService, adminService, athleteService, config, iterationManager);

		var iteration = new Iteration { Series = new Series { Key = "UTs", Name = "Unit Tests" } };
		iterationManager.ActiveIteration().Returns(iteration);

		//act
		var result = await component.InvokeAsync();

		//assert
		var model = result.ViewData!.Model as HeaderViewModel;
		Assert.Equal("Unit Tests", model!.Series.Name);
	}
}