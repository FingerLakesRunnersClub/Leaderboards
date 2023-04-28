using System.Collections.Immutable;
using System.Text.Json;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Metrics;
using FLRC.Leaderboards.Core.Races;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Data;

public sealed class UltraSignupTests
{
	[Fact]
	public async Task CanGetResultsForCourse()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/ultra.json");
		var json = JsonDocument.Parse(data).RootElement;
		var course = new Course { Race = new Race { Date = new DateTime(2011, 9, 24) } };
		var source = new UltraSignup(TestHelpers.Config);

		//act
		var results = source.ParseCourse(course, json, ImmutableDictionary<string, string>.Empty);
		var result = results.First();

		//assert
		Assert.Equal((ushort) 234, result.Athlete.ID);
		Assert.Equal("Steve Desmond", result.Athlete.Name);
		Assert.Equal(26, result.Athlete.Age);
		Assert.Equal(AgeGradeCalculator.Category.M, result.Athlete.Category.Value);
		Assert.Equal(new DateTime(2011, 09, 24), result.StartTime.Value);
		Assert.Equal(new Time(new TimeSpan(0, 5, 04, 0)), result.Duration);
	}

	[Fact]
	public async Task CanGetResultsWithAlias()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/ultra.json");
		var json = JsonDocument.Parse(data).RootElement;
		var course = new Course { Race = new Race { Date = new DateTime(2011, 9, 24) } };
		var source = new UltraSignup(TestHelpers.Config);
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
	public async Task CanParseAthlete()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/ultra.json");
		var json = JsonDocument.Parse(data).RootElement;
		var source = new UltraSignup(TestHelpers.Config);

		//act
		var athlete = source.ParseAthlete(json.EnumerateArray().First(), ImmutableDictionary<string, string>.Empty);

		//assert
		Assert.Equal((ushort) 234, athlete.ID);
		Assert.Equal("Steve Desmond", athlete.Name);
		Assert.Equal(26, athlete.Age);
		Assert.Equal(AgeGradeCalculator.Category.M, athlete.Category.Value);
	}

	[Fact]
	public async Task CanParseAthleteWithAlias()
	{
		//arrange
		var data = await File.ReadAllTextAsync("json/ultra.json");
		var json = JsonDocument.Parse(data).RootElement;
		var source = new UltraSignup(TestHelpers.Config);
		var aliases = new Dictionary<string, string>
		{
			{ "Steve Desmond", "Rob Sutherland" }
		};

		//act
		var athlete = source.ParseAthlete(json.EnumerateArray().First(), aliases);

		//assert
		Assert.Equal((ushort) 234, athlete.ID);
		Assert.Equal("Rob Sutherland", athlete.Name);
	}

	[Fact]
	public void CanParseDuration()
	{
		//arrange
		const string ms = "123120";

		//act
		var duration = UltraSignup.ParseDuration(ms);

		//assert
		Assert.Equal(new TimeSpan(0, 0, 2, 3, 120), duration.Value);
	}

	[Fact]
	public void CanParseURL()
	{
		//arrange
		var source = new UltraSignup(TestHelpers.Config);

		//act
		var url = source.URL(123);

		//assert
		Assert.StartsWith("https://ultrasignup.com/", url);
		Assert.EndsWith("/123/1/json", url);
	}
}