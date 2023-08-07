using System.Text.Json;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Data;

public sealed class DataServiceTests
{
	[Fact]
	public async Task CanParseResultsFromAPI()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var course = await dataService.GetResults(123, null);

		//assert
		Assert.Equal("Steve Desmond", course.Results.First().Athlete.Name);
	}

	[Fact]
	public async Task CanGetAllResults()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var allResults = await dataService.GetAllResults();

		//assert
		Assert.Equal("Steve Desmond", allResults.First().Results.First().Athlete.Name);
	}

	[Fact]
	public async Task ResultsFromAPIAreCached()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetResults(123, null);
		await dataService.GetResults(123, null);

		//assert
		await resultsAPI[Arg.Any<string>()].Received(1).GetResults(Arg.Any<uint>());
	}

	[Fact]
	public async Task ResultsCombineAliases()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));

		var resultsJSON = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(resultsJSON.RootElement);

		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var aliases = new Dictionary<string, string>
		{
			{ "Steve Desmond", "Rob Sutherland" }
		};
		customInfoAPI.GetAliases().Returns(aliases);

		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var course = await dataService.GetResults(123, null);

		//assert
		var result = course.Results.Single();
		Assert.Equal("Rob Sutherland", result.Athlete.Name);
	}

	[Fact]
	public async Task ResultsIncludeCommunityStars()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var resultsJSON = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(resultsJSON.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var posts = new List<Post>
		{
			new() { Name = "Steve Desmond", Date = new DateTime(2011, 9, 24), Content = "## Story" }
		};
		communityAPI.ParsePosts(Arg.Any<IReadOnlyCollection<JsonElement>>()).Returns(posts);
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var course = await dataService.GetResults(123, null);

		//assert
		var result = course.Results.Single();
		Assert.False(result.CommunityStars[StarType.GroupRun]);
		Assert.True(result.CommunityStars[StarType.Story]);
	}

	[Fact]
	public async Task CommunityStarsExceptionIsLogged()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var resultsJSON = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(resultsJSON.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		communityAPI.ParsePosts(Arg.Any<IReadOnlyCollection<JsonElement>>()).Throws(new Exception());
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetResults(123, null);

		//assert
		Assert.True(logger.Called);
	}

	[Fact]
	public async Task ResultsAPIExceptionsAreLogged()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Throws(new Exception());
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetResults(123, null);

		//assert
		Assert.True(logger.Called);
	}

	[Fact]
	public async Task CanGetAthletesFromResults()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var athletes = await dataService.GetAthletes();

		//assert
		Assert.Equal("Steve Desmond", athletes.ToArray().First().Value.Name);
	}

	[Fact]
	public async Task CanGetAthletesFromStartList()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder()
			.AddJsonFile("json/config.json")
			.AddInMemoryCollection(new Dictionary<string, string> { { "StartListRaceID", "123" } })
			.Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var athletes = await dataService.GetAthletes();

		//assert
		Assert.Equal("Steve Desmond", athletes.ToArray().First().Value.Name);
	}

	[Fact]
	public async Task CanParseAthleteFromAPI()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var athlete = await dataService.GetAthlete(234);

		//assert
		Assert.Equal("Steve Desmond", athlete.Name);
	}

	[Fact]
	public async Task AthletesFromAPIAreCached()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetAthlete(234);
		await dataService.GetAthlete(234);

		//assert
		await resultsAPI[Arg.Any<string>()].Received(1).GetResults(Arg.Any<uint>());
	}

	[Fact]
	public async Task AthleteAPIExceptionsAreLogged()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Throws(new Exception());
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetAthlete(234);

		//assert
		Assert.True(logger.Called);
	}

	private static IDictionary<string, IReadOnlyCollection<uint>> groups
		=> new Dictionary<string, IReadOnlyCollection<uint>>
		{
			{ "Test 1", new uint[] { 123, 234 } },
			{ "Test 2", new uint[] { 234, 345 } }
		};

	[Fact]
	public async Task CanGetGroupMembers()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var athleteJSON = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(athleteJSON.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		customInfoAPI.GetGroups().Returns(groups);
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var members = await dataService.GetGroupMembers("Test 1");

		//assert
		Assert.Equal("Steve Desmond", members.First().Name);
	}

	[Fact]
	public async Task GroupMembersAreCached()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var athleteJSON = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(athleteJSON.RootElement);
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		customInfoAPI.GetGroups().Returns(groups);
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var members1 = await dataService.GetGroupMembers("Test 1");
		var members2 = await dataService.GetGroupMembers("Test 2");

		//assert
		Assert.Equal("Steve Desmond", members1.First().Name);
		Assert.Equal("Steve Desmond", members2.First().Name);
		await customInfoAPI.Received(1).GetGroups();
	}

	[Fact]
	public async Task GroupMemberExceptionsAreLogged()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		customInfoAPI.GetGroups().Throws(new Exception());
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetGroupMembers("Test 1");

		//assert
		Assert.True(logger.Called);
	}

	[Fact]
	public async Task CanGetCommunityUsers()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		communityAPI.GetUsers().Returns(new[]
		{
			JsonDocument.Parse(@"{""id"":123,""name"":""Steve"",""username"":""steve""}").RootElement,
			JsonDocument.Parse(@"{""id"":234,""name"":""Test"",""username"":""test""}").RootElement
		});
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var users = await dataService.GetCommunityUsers();

		//assert
		Assert.Equal(2, users.Count);
		Assert.Equal("Steve", users.First().Name);
		Assert.Equal("Test", users.Skip(1).First().Name);
	}

	[Fact]
	public async Task CanGetCommunityGroupMembers()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		communityAPI.GetMembers("group").Returns(new[]
		{
			JsonDocument.Parse(@"{""id"":123,""name"":""Steve"",""username"":""steve""}").RootElement,
			JsonDocument.Parse(@"{""id"":234,""name"":""Test"",""username"":""test""}").RootElement
		});
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		//act
		var users = await dataService.GetCommunityGroupMembers("group");

		//assert
		Assert.Equal(2, users.Count);
		Assert.Equal("Steve", users.First().Name);
		Assert.Equal("Test", users.Skip(1).First().Name);
	}

	[Fact]
	public async Task CanAddMembersToGroups()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		var customInfoAPI = Substitute.For<ICustomInfoAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		communityAPI.GetGroup("group1").Returns(JsonDocument.Parse(@"{""id"":123,""name"":""group1""}").RootElement);
		communityAPI.GetGroup("group2").Returns(JsonDocument.Parse(@"{""id"":234,""name"":""group2""}").RootElement);
		var arg1 = Array.Empty<string>();
		var arg2 = Array.Empty<string>();
		await communityAPI.AddMembers(123, Arg.Do<IReadOnlyCollection<string>>(c => arg1 = c.ToArray()));
		await communityAPI.AddMembers(234, Arg.Do<IReadOnlyCollection<string>>(c => arg2 = c.ToArray()));
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, customInfoAPI, communityAPI, config, loggerFactory);

		var members = new Dictionary<string, string[]>
		{
			{ "group1", new[] { "user1", "user2" } },
			{ "group2", new[] { "user1", "user3" } }
		};

		//act
		await dataService.AddCommunityGroupMembers(members);

		//assert
		Assert.Contains("user1", arg1);
		Assert.Contains("user2", arg1);
		Assert.DoesNotContain("user3", arg1);

		Assert.Contains("user1", arg2);
		Assert.DoesNotContain("user2", arg2);
		Assert.Contains("user3", arg2);
	}
}