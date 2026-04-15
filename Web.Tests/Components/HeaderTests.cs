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
		var seriesService = Substitute.For<ISeriesService>();
		seriesService.Find("UTs").Returns(new Series { Key = "UTs", Name = "Unit Tests" });

		var settings = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var config = new AppConfig(settings);

		var app = new AppContextProvider("UTs");

		var component = new Header(authService, adminService, athleteService, seriesService, config, app);

		//act
		var result = await component.InvokeAsync();

		//assert
		var model = (HeaderViewModel)result.ViewData!.Model;
		Assert.Equal("Unit Tests", model!.Series.Name);
	}
}