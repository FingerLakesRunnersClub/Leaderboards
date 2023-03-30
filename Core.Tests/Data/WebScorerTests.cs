using System.Collections.Immutable;
using System.Text.Json;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Data;

public class WebScorerTests
{
	[Fact]
	public async Task CanGetResultsForCourse()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/athlete.json");
		var json = JsonDocument.Parse(data).RootElement;
		var course = new Course();
		var source = new WebScorer(TestHelpers.Config);

		//act
		var results = source.ParseCourse(course, json, ImmutableDictionary<string, string>.Empty);
		var result = results.First();

		//assert
		Assert.Equal((ushort) 234, result.Athlete.ID);
		Assert.Equal("Steve Desmond", result.Athlete.Name);
		Assert.Equal(26, result.Athlete.Age);
		Assert.Equal(new DateTime(1985, 02, 16), result.Athlete.DateOfBirth);
		Assert.Equal(AgeGradeCalculator.Category.M, result.Athlete.Category.Value);
		Assert.Equal(new DateTime(2011, 09, 24), result.StartTime.Value);
		Assert.Equal(new Time(new TimeSpan(0, 5, 04, 0)), result.Duration);
	}

	[Fact]
	public async Task CanGetResultsWithAlias()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/athlete.json");
		var json = JsonDocument.Parse(data).RootElement;
		var course = new Course();
		var source = new WebScorer(TestHelpers.Config);
		var aliases = new Dictionary<string, string>
		{
			{ "Steve Desmond", "Rob Sutherland" }
		};

		//act
		var results = source.ParseCourse(course, json, aliases);
		var result = results.First();

		//assert
		Assert.Equal((ushort) 234, result.Athlete.ID);
		Assert.Equal("Rob Sutherland", result.Athlete.Name);
	}

	[Fact]
	public async Task DNFsAreIgnored()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/dnf.json");
		var json = JsonDocument.Parse(data).RootElement;
		var course = new Course();
		var source = new WebScorer(TestHelpers.Config);

		//act
		var results = source.ParseCourse(course, json, ImmutableDictionary<string, string>.Empty);

		//assert
		Assert.Empty(results);
	}

	[Fact]
	public async Task FalseStartsAreIgnored()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/false-start.json");
		var json = JsonDocument.Parse(data).RootElement;
		var course = new Course();
		var source = new WebScorer(TestHelpers.Config);

		//act
		var results = source.ParseCourse(course, json, ImmutableDictionary<string, string>.Empty);

		//assert
		Assert.Empty(results);
	}

	[Fact]
	public async Task PrivateAthletesHaveNoTime()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/private.json");
		var json = JsonDocument.Parse(data).RootElement;
		var course = new Course();
		var source = new WebScorer(TestHelpers.Config);

		//act
		var results = source.ParseCourse(course, json, ImmutableDictionary<string, string>.Empty);

		//assert
		Assert.Null(results.First().Duration);
	}

	[Fact]
	public async Task CanParseAthlete()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/athlete.json");
		var json = JsonDocument.Parse(data).RootElement;
		var element = json.GetProperty("Racers").EnumerateArray().First();
		var source = new WebScorer(TestHelpers.Config);

		//act
		var athlete = source.ParseAthlete(element, ImmutableDictionary<string, string>.Empty);

		//assert
		Assert.Equal((ushort) 234, athlete.ID);
		Assert.Equal("Steve Desmond", athlete.Name);
		Assert.Equal(26, athlete.Age);
		Assert.Equal(AgeGradeCalculator.Category.M, athlete.Category.Value);
		Assert.Equal(new DateTime(1985, 02, 16), athlete.DateOfBirth);
	}

	[Fact]
	public async Task CanParsePrivateAthlete()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/private.json");
		var json = JsonDocument.Parse(data).RootElement;
		var element = json.GetProperty("Racers").EnumerateArray().First();
		var source = new WebScorer(TestHelpers.Config);

		//act
		var athlete = source.ParseAthlete(element, ImmutableDictionary<string, string>.Empty);

		//assert
		Assert.Equal((ushort) 234, athlete.ID);
		Assert.Equal("Steve Desmond", athlete.Name);
		Assert.Equal(26, athlete.Age);
		Assert.Equal(AgeGradeCalculator.Category.M, athlete.Category.Value);
		Assert.Equal(new DateTime(1985, 02, 16), athlete.DateOfBirth);
		Assert.True(athlete.Private);
	}

	[Fact]
	public async Task CanParseAthleteWithAlias()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/athlete.json");
		var json = JsonDocument.Parse(data).RootElement;
		var element = json.GetProperty("Racers").EnumerateArray().First();
		var source = new WebScorer(TestHelpers.Config);
		var aliases = new Dictionary<string, string>
		{
			{ "Steve Desmond", "Rob Sutherland" }
		};

		//act
		var athlete = source.ParseAthlete(element, aliases);

		//assert
		Assert.Equal((ushort) 234, athlete.ID);
		Assert.Equal("Rob Sutherland", athlete.Name);
	}

	[Fact]
	public void CanParseDuration()
	{
		//arrange
		const double seconds = 123.12;

		//act
		var duration = WebScorer.ParseDuration(seconds);

		//assert
		Assert.Equal(new TimeSpan(0, 2, 4), duration.Value);
	}

	[Fact]
	public void DurationRoundsAppropriately()
	{
		//arrange
		const double seconds = 123.09;

		//act
		var duration = WebScorer.ParseDuration(seconds);

		//assert
		Assert.Equal(new TimeSpan(0, 2, 3), duration.Value);
	}
}