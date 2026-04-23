using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class IterationManagerTests
{
	[Fact]
	public async Task CanGetActiveIteration()
	{
		//arrange
		var contextProvider = Substitute.For<IContextManager>();
		var iterationService = Substitute.For<IIterationService>();

		var manager = new IterationManager(contextProvider, iterationService);

		var id = Guid.NewGuid();
		contextProvider.Series().Returns(new Series { ID = id });
		iterationService.Current(id).Returns(new Iteration { Name = "Test" });

		//act
		var iteration = await manager.ActiveIteration();

		//assert
		Assert.Equal("Test", iteration!.Name);
	}
}