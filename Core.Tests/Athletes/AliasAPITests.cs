using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Config;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Athletes;

public class AliasAPITests
{
	[Fact]
	public async Task CanCreateAliasLookup()
	{
		//arrange
		var json = @"{ ""Incorrect One"": ""Correct Un"", ""Incorrect Two"": ""Correct Deux"" }";
		var http = new MockHttpMessageHandler(json);
		var config = Substitute.For<IConfig>();
		config.AliasAPI.Returns("http://localhost");

		var api = new AliasAPI(new HttpClient(http), config);

		//act
		var aliases = await api.GetAliases();

		//assert
		Assert.Equal("Correct Un", aliases["Incorrect One"]);
		Assert.Equal("Correct Deux", aliases["Incorrect Two"]);
	}

	[Fact]
	public async Task AliasesAreEmptyWhenNoAPI()
	{
		//arrange
		var json = @"{ ""Incorrect One"": ""Correct Un"", ""Incorrect Two"": ""Correct Deux"" }";
		var http = new MockHttpMessageHandler(json);
		var config = Substitute.For<IConfig>();

		var api = new AliasAPI(new HttpClient(http), config);

		//act
		var aliases = await api.GetAliases();

		//assert
		Assert.Empty(aliases);
	}
}