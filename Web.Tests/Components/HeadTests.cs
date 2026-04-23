using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using FLRC.Leaderboards.Services;
using FLRC.Leaderboards.Web.Components;
using FLRC.Leaderboards.Web.ViewModels;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests.Components;

public sealed class HeadTests
{
	[Fact]
	public async Task CanRenderComponent()
	{
		//arrange
		var contextManager = Substitute.For<IContextManager>();
		var component = new Head(contextManager);

		contextManager.Series().Returns(new Series());
		
		//act
		var result = await component.InvokeAsync("test page");

		//assert
		var model = (HeadViewModel)result.ViewData!.Model;
		Assert.Equal("test page", model!.PageTitle);
	}
}