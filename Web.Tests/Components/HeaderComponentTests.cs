using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using FLRC.Leaderboards.Web.Components;
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

		var settings = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var config = new AppConfig(settings);

		var context = Substitute.For<IHttpContextAccessor>();

		var component = new HeaderComponent(seriesService, context, config);

		//act
		var result = await component.InvokeAsync();

		//assert
		var model = (AppConfig)result.ViewData!.Model;
		Assert.Equal("Unit Tests", model!.App);
	}
}