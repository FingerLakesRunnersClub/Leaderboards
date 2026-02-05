using FLRC.Leaderboards.Core.Config;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Config;

public sealed class AppContextProviderTests
{
	[Fact]
	public void CanGetAppContext()
	{
		//arrange
		var provider = new AppContextProvider("test");

		//act
		var context = provider.App;

		//assert
		Assert.Equal("test", context);
	}

	[Fact]
	public void CanGetContextAsString()
	{
		//arrange
		var provider = new AppContextProvider("test");

		//act
		var context = provider.ToString();

		//assert
		Assert.Equal("test", context);
	}
}