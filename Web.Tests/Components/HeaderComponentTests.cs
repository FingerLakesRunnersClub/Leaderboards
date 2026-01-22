using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Web.Components;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class HeaderComponentTests
{
	[Fact]
	public void CanRenderComponent()
	{
		//arrange
		var settings = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var config = new AppConfig(settings);

		var component = new HeaderComponent(config);

		//act
		var result = component.Invoke();

		//assert
		var model = (AppConfig)result.ViewData!.Model;
		Assert.Equal("Unit Tests", model!.App);
	}
}