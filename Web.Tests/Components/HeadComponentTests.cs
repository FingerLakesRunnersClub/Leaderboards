using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Web.Areas.Admin.Services;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class HeadComponentTests
{
	[Fact]
	public async Task CanRenderComponent()
	{
		//arrange
		var seriesService = Substitute.For<ISeriesService>();
		seriesService.FindSeries("test").Returns(new Series());

		var context = new AppContextProvider("test");
		var component = new HeadComponent(seriesService, context);

		//act
		var result = await component.InvokeAsync("test page");

		//assert
		var model = (HeadViewModel)result.ViewData!.Model;
		Assert.Equal("test page", model!.PageTitle);
	}
}