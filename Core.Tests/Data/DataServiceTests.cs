using System.Text.Json;
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
	public void CanGetCourseNames()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		var groupAPI = Substitute.For<IGroupAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		var courseNames = dataService.CourseNames;

		//assert
		Assert.Equal("Virgil Crest Ultramarathons", courseNames.First().Value);
	}

	[Fact]
	public async Task CanParseResultsFromAPI()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		dataAPI[Arg.Any<string>()].Source.Returns(new WebScorer());
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		var course = await dataService.GetResults(123);

		//assert
		Assert.Equal("Steve Desmond", course.Results.First().Athlete.Name);
	}

	[Fact]
	public async Task CanGetAllResults()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		dataAPI[Arg.Any<string>()].Source.Returns(new WebScorer());
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		var allResults = await dataService.GetAllResults();

		//assert
		Assert.Equal("Steve Desmond", allResults.First().Results.First().Athlete.Name);
	}

	[Fact]
	public async Task ResultsFromAPIAreCached()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		await dataService.GetResults(123);
		await dataService.GetResults(123);

		//assert
		await dataAPI[Arg.Any<string>()].Received(1).GetResults(Arg.Any<uint>());
	}

	[Fact]
	public async Task ResultsAPIExceptionsAreLogged()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Throws(new Exception());
		var groupAPI = Substitute.For<IGroupAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		await dataService.GetResults(123);

		//assert
		Assert.True(logger.Called);
	}

	[Fact]
	public async Task CanParseAthleteFromAPI()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		dataAPI[Arg.Any<string>()].Source.Returns(new WebScorer());
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		var athlete = await dataService.GetAthlete(234);

		//assert
		Assert.Equal("Steve Desmond", athlete.Name);
	}

	[Fact]
	public async Task CanGetAllAthletes()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		dataAPI[Arg.Any<string>()].Source.Returns(new WebScorer());
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		var athletes = await dataService.GetAthletes();

		//assert
		Assert.Equal("Steve Desmond", athletes.ToList().First().Value.Name);
	}

	[Fact]
	public async Task AthletesFromAPIAreCached()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		var json = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(json.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		await dataService.GetAthlete(234);
		await dataService.GetAthlete(234);

		//assert
		await dataAPI[Arg.Any<string>()].Received(1).GetResults(Arg.Any<uint>());
	}

	[Fact]
	public async Task AthleteAPIExceptionsAreLogged()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Throws(new Exception());
		var groupAPI = Substitute.For<IGroupAPI>();
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		await dataService.GetAthlete(234);

		//assert
		Assert.True(logger.Called);
	}

	private IDictionary<string, IEnumerable<uint>> groups
		=> new Dictionary<string, IEnumerable<uint>>
		{
				{"Test 1", new uint[] {123, 234}},
				{"Test 2", new uint[] {234, 345}}
		};

	[Fact]
	public async Task CanGetGroupMembers()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		dataAPI[Arg.Any<string>()].Source.Returns(new WebScorer());
		var athleteJSON = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(athleteJSON.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		groupAPI.GetGroups().Returns(groups);
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		var members = await dataService.GetGroupMembers("Test 1");

		//assert
		Assert.Equal("Steve Desmond", members.First().Name);
	}

	[Fact]
	public async Task GroupMembersAreCached()
	{
		//arrange
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		dataAPI[Arg.Any<string>()].Source.Returns(new WebScorer());
		var athleteJSON = await JsonDocument.ParseAsync(File.OpenRead("json/athlete.json"));
		dataAPI[Arg.Any<string>()].GetResults(Arg.Any<uint>()).Returns(athleteJSON.RootElement);
		var groupAPI = Substitute.For<IGroupAPI>();
		groupAPI.GetGroups().Returns(groups);
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

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
		var dataAPI = Substitute.For<IDictionary<string,IDataAPI>>();
		var groupAPI = Substitute.For<IGroupAPI>();
		groupAPI.GetGroups().Throws(new Exception());
		var config = new ConfigurationBuilder().AddJsonFile("json/config.json").Build();
		var logger = new TestLogger();
		var loggerFactory = Substitute.For<ILoggerFactory>();
		loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
		var dataService = new DataService(dataAPI, groupAPI, config, loggerFactory);

		//act
		await dataService.GetGroupMembers("Test 1");

		//assert
		Assert.True(logger.Called);
	}
}