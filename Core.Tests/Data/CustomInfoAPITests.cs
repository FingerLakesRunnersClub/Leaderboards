using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Data;

public sealed class AliasAPITests
{
	[Fact]
	public async Task CanCreateAliasLookup()
	{
		//arrange
		const string json = """{ "Incorrect One": "Correct Un", "Incorrect Two": "Correct Deux" }""";
		var http = new MockHttpMessageHandler(json);
		var config = Substitute.For<IConfig>();
		config.AliasAPI.Returns("http://localhost");

		var api = new CustomInfoAPI(new HttpClient(http), config);

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
		const string json = """{ "Incorrect One": "Correct Un", "Incorrect Two": "Correct Deux" }""";
		var http = new MockHttpMessageHandler(json);
		var config = Substitute.For<IConfig>();

		var api = new CustomInfoAPI(new HttpClient(http), config);

		//act
		var aliases = await api.GetAliases();

		//assert
		Assert.Empty(aliases);
	}

	[Fact]
	public async Task CanGetGroupMemberIDs()
	{
		//arrange
		const string data = """{ "Test 1": [ 123, 234 ], "Test 2": [ 234, 345 ] }""";
		var http = new MockHttpMessageHandler(data);
		var config = Substitute.For<IConfig>();
		config.GroupAPI.Returns("http://localhost");
		var api = new CustomInfoAPI(new HttpClient(http), config);

		//act
		var groups = await api.GetGroups();

		//assert
		var members1 = groups["Test 1"];
		Assert.Equal((uint)123, members1[0]);
		Assert.Equal((uint)234, members1[1]);

		var members2 = groups["Test 2"];
		Assert.Equal((uint)234, members2[0]);
		Assert.Equal((uint)345, members2[1]);
	}

	[Fact]
	public async Task GroupMembersAreEmptyWhenNoAPI()
	{
		//arrange
		const string data = """{ "Test 1": [ 123, 234 ], "Test 2": [ 234, 345 ] }""";
		var http = new MockHttpMessageHandler(data);
		var config = Substitute.For<IConfig>();
		var api = new CustomInfoAPI(new HttpClient(http), config);

		//act
		var groups = await api.GetGroups();

		//assert
		Assert.Empty(groups);
	}

	[Fact]
	public async Task CanGetPersonalCompletions()
	{
		//arrange
		const string data = """{ "123": "2023-08-09" }""";
		var http = new MockHttpMessageHandler(data);
		var config = Substitute.For<IConfig>();
		config.PersonalAPI.Returns("http://localhost");
		var api = new CustomInfoAPI(new HttpClient(http), config);

		//act
		var personal = await api.GetPersonalCompletions();

		//assert
		Assert.Equal(new DateOnly(2023, 08, 09), personal[123]);
	}

	[Fact]
	public async Task PersonalCompletionsAreEmptyWhenNoAPI()
	{
		//arrange
		const string data = """{ "123": "2023-08-09" }""";
		var http = new MockHttpMessageHandler(data);
		var config = Substitute.For<IConfig>();
		var api = new CustomInfoAPI(new HttpClient(http), config);

		//act
		var personal = await api.GetPersonalCompletions();

		//assert
		Assert.Empty(personal);
	}
}