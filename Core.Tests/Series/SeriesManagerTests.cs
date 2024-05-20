using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Data;
using FLRC.Leaderboards.Core.Series;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Series;

public sealed class SeriesManagerTests
{
	[Fact]
	public async Task CanGetEarliestResults()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var courses = SeriesData.Results.Select(r => r.Course).Distinct().ToArray();
		foreach (var course in courses)
			course.Results = SeriesData.Results.Where(r => r.Course == course).ToArray();
		dataService.GetAllResults().Returns(courses);

		var settings = new ConfigurationBuilder().AddJsonFile("json/series.json").Build();
		var config = new AppConfig(settings);
		var manager = new SeriesManager(dataService, config);

		//act
		var results = await manager.Earliest();

		//assert
		Assert.Equal(3, results.Count);

		var ultraResults = results[config.Series["100K"]];
		Assert.Equal(3, ultraResults.Count);
		Assert.Equal(CourseData.Athlete2, ultraResults[0].Value.Athlete);
		Assert.Equal(CourseData.Athlete1, ultraResults[1].Value.Athlete);
		Assert.Equal(CourseData.Athlete1, ultraResults[2].Value.Athlete);
	}

	[Fact]
	public async Task CanGetFastestResults()
	{
		//arrange
		var dataService = Substitute.For<IDataService>();
		var courses = SeriesData.Results.Select(r => r.Course).Distinct().ToArray();
		foreach (var course in courses)
			course.Results = SeriesData.Results.Where(r => r.Course == course).ToArray();
		dataService.GetAllResults().Returns(courses);

		var settings = new ConfigurationBuilder().AddJsonFile("json/series.json").Build();
		var config = new AppConfig(settings);
		var manager = new SeriesManager(dataService, config);

		//act
		var results = await manager.Fastest();

		//assert
		Assert.Equal(3, results.Count);

		var ultraResults = results[config.Series["100K"]];
		Assert.Equal(3, ultraResults.Count);
		Assert.Equal(CourseData.Athlete1, ultraResults[0].Value.Athlete);
		Assert.Equal(CourseData.Athlete2, ultraResults[1].Value.Athlete);
		Assert.Equal(CourseData.Athlete1, ultraResults[2].Value.Athlete);
	}
}