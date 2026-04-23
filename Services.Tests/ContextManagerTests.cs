using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class ContextManagerTests
{
    [Fact]
    public async Task CanGetSeries()
    {
        //arrange
        var contextProvider = Substitute.For<IContextProvider>();
        var seriesService = Substitute.For<ISeriesService>();
        var contextManager = new ContextManager(contextProvider, seriesService);

        var id = Guid.NewGuid();
        contextProvider.App.Returns("test");
        seriesService.Find("test").Returns(new Series { ID = id });

        //act
        var series = await contextManager.Series();

        //assert
        Assert.Equal(id, series.ID);
    }

    [Fact]
    public async Task ThrowsIfMisconfigured()
    {
        //arrange
        var contextProvider = Substitute.For<IContextProvider>();
        var seriesService = Substitute.For<ISeriesService>();
        var contextManager = new ContextManager(contextProvider, seriesService);

        var id = Guid.NewGuid();
        contextProvider.App.Returns("test1");
        seriesService.Find("test2").Returns(new Series { ID = id });

        //act
        var task = contextManager.Series();

        //assert
        await Assert.ThrowsAsync<NullReferenceException>(async () => await task);
    }
}