using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public class IterationManagerTests
{
	[Fact]
	public async Task CanGetActiveIteration()
	{
		//arrange
		var contextProvider = Substitute.For<IContextProvider>();
		var iterationService = Substitute.For<IIterationService>();
		var seriesService = Substitute.For<ISeriesService>();

		var manager = new IterationManager(contextProvider, iterationService, seriesService);

		var id = Guid.NewGuid();
		contextProvider.App.Returns("tests");
		seriesService.Find("tests").Returns(new Series { ID = id });
		iterationService.Current(id).Returns(new Iteration { Name = "Test" });

		//act
		var iteration = await manager.ActiveIteration();

		//assert
		Assert.Equal("Test", iteration!.Name);
	}
}