using System.Text.Json;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Groups;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Data;

public class DataServiceTests
{
	[Fact]
	public async Task CanParseResultsFromAPI()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

		//act
		var course = await dataService.GetResults(123);

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
		var groupAPI = Substitute.For<IGroupAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

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
		var groupAPI = Substitute.For<IGroupAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		communityAPI.GetPosts(Arg.Any<ushort>()).Returns(JsonDocument.Parse("[]").RootElement);
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetResults(123);
		await dataService.GetResults(123);

		//assert
		await resultsAPI[Arg.Any<string>()].Received(1).GetResults(Arg.Any<uint>());
	}

	[Fact]
	public async Task ResultsIncludeCommunityStars()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var resultsJSON = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(resultsJSON.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var posts = new List<Post>
		{
			new() { Name = "Steve Desmond", Date = new DateTime(2011, 9, 24), Content = "## Story" },
			new() { Name = "Other Athlete", Date = new DateTime(2011, 9, 24), Content = "## Shop Local" },
			new() { Name = "Steve Desmond", Date = new DateTime(2011, 9, 25), Content = "## Shop Local" },
		};
		communityAPI.ParsePosts(Arg.Any<JsonElement>()).Returns(posts);
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

		//act
		var course = await dataService.GetResults(123);

		//assert
		var result = course.Results.Single();
		Assert.False(result.CommunityStars[StarType.GroupRun]);
		Assert.True(result.CommunityStars[StarType.Story]);
		Assert.False(result.CommunityStars[StarType.ShopLocal]);
	}

	[Fact]
	public async Task ResultsAPIExceptionsAreLogged()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Throws(new Exception());
		var groupAPI = Substitute.For<IGroupAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetResults(123);

		//assert
		Assert.True(logger.Called);
	}

	[Fact]
	public async Task CanParseAthleteFromAPI()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

		//act
		var athlete = await dataService.GetAthlete(234);

		//assert
		Assert.Equal("Steve Desmond", athlete.Name);
	}

	[Fact]
	public async Task CanGetAllAthletes()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		resultsAPI[Arg.Any<string>()].Source.Returns(new WebScorer(TestHelpers.Config));
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

		//act
		var athletes = await dataService.GetAthletes();

		//assert
		Assert.Equal("Steve Desmond", athletes.ToList().First().Value.Name);
	}

	[Fact]
	public async Task AthletesFromAPIAreCached()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		resultsAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

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
		var groupAPI = Substitute.For<IGroupAPI>();
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetAthlete(234);

		//assert
		Assert.True(logger.Called);
	}

	private IDictionary<string, IReadOnlyCollection<uint>> groups
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
		var groupAPI = Substitute.For<IGroupAPI>();
		groupAPI.GetGroups().Returns(groups);
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

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
		var groupAPI = Substitute.For<IGroupAPI>();
		groupAPI.GetGroups().Returns(groups);
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

		//act
		var members1 = await dataService.GetGroupMembers("Test 1");
		var members2 = await dataService.GetGroupMembers("Test 2");

		//assert
		Assert.Equal("Steve Desmond", members1.First().Name);
		Assert.Equal("Steve Desmond", members2.First().Name);
		await groupAPI.Received(1).GetGroups();
	}

	[Fact]
	public async Task GroupMemberExceptionsAreLogged()
	{
		//arrange
		var resultsAPI = Substitute.For<IDictionary<string, IResultsAPI>>();
		var groupAPI = Substitute.For<IGroupAPI>();
		groupAPI.GetGroups().Throws(new Exception());
		var communityAPI = Substitute.For<ICommunityAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(resultsAPI, groupAPI, communityAPI, config, loggerFactory);

		//act
		await dataService.GetGroupMembers("Test 1");

		//assert
		Assert.True(logger.Called);
	}
}