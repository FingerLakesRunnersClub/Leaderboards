using FLRC.Leaderboards.Model;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class DiscourseFactoryTests
{
    [Fact]
    public async Task CanCreateAuthenticator()
    {
        //arrange
        var contextManager = Substitute.For<IContextManager>();
        var factory = new DiscourseFactory(contextManager);

        var id = Guid.NewGuid();
        var series = new Series
        {
            ID = id,
            Settings =
            [
                new Setting { SeriesID = id, Key = "CommunityURL", Value = "https://example.com" },
                new Setting { SeriesID = id, Key = "DiscourseAuthSecret", Value = "abc123" }
            ]
        };
        contextManager.Series().Returns(series);

        //act
        var discourse = await factory.Authenticator();

        //assert
        Assert.NotNull(discourse);
    }
}