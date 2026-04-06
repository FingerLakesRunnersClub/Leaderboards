using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.ViewModels;
using Microsoft.AspNetCore.Http;
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
		var iterationManager = Substitute.For<IIterationManager>();
		var iteration = new Iteration { Series = new Series { Key = "UTs", Name = "Unit Tests" } };
		iterationManager.ActiveIteration().Returns(iteration);

		var http = Substitute.For<IHttpContextAccessor>();

		var settings = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var config = new AppConfig(settings);

		var component = new Header(iterationManager, http, config);

		//act
		var result = await component.InvokeAsync();

		//assert
		var model = result.ViewData!.Model as HeaderViewModel;
		Assert.Equal("Unit Tests", model!.Series.Name);
	}
}