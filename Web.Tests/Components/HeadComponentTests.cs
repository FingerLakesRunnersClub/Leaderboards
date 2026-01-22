using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class HeadComponentTests
{
	[Fact]
	public void CanRenderComponent()
	{
		//arrange
		var settings = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var config = new AppConfig(settings);

		var component = new HeadComponent(config);

		//act
		var result = component.Invoke("test page");

		//assert
		var model = (HeadViewModel)result.ViewData!.Model;
		Assert.Equal("test page", model!.PageTitle);
	}
}