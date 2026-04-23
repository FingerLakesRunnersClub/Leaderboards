using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Services.Tests;

public sealed class WebScorerStartListTests
{
	[Fact]
	public void URLContainsAllParams()
	{
		//arrange
		var contextManager = Substitute.For<IContextManager>();
		var source = new WebScorerStartList(contextManager);

		var id = Guid.NewGuid();
		var series = new Series
		{
			ID = id,
			Settings =
			[
				new Setting { SeriesID = id, Key = nameof(IConfig.WebScorerOrg), Value = "456" },
				new Setting { SeriesID = id, Key = nameof(IConfig.WebScorerSecret), Value = "789" }
			]
		};
		contextManager.Series().Returns(series);

		//act
		var url = source.URL(123);

		//assert
		Assert.Contains("123", url);
		Assert.Contains("456", url);
		Assert.Contains("789", url);
	}
}