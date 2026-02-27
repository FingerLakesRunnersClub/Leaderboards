using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class HeaderComponentTests
{
	[Fact]
	public async Task CanRenderComponent()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		seriesService.FindSeries("UTs").Returns(new Series { Key = "UTs", Name = "Unit Tests" });

		var http = Substitute.For<IHttpContextAccessor>();

		var settings = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var config = new AppConfig(settings);

		var app = new AppContextProvider("UTs");

		var component = new HeaderComponent(seriesService, http, config, app);

		//act
		var result = await component.InvokeAsync();

		//assert
		var model = (HeaderViewModel)result.ViewData!.Model;
		Assert.Equal("Unit Tests", model!.Series.Name);
	}
}