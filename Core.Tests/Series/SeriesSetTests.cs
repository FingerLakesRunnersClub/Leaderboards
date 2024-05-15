using FLRC.Leaderboards.Core.Series;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Series;

public sealed class SeriesSetTests
{
	[Fact]
	public void CanCreateSeriesSetFromConfig()
	{
		//arrange
		var config = new ConfigurationBuilder().AddJsonFile("json/series.json").Build();

		//act
		var set = new SeriesSet(config.GetSection("Series"));

		//assert
		Assert.Equal(3, set.Count);

		Assert.Equal("100K", set[0].ID);
		Assert.Equal("Test 1", set[0].Name);
		Assert.Equal(20, set[0].HourLimit);
		Assert.Equal([123, 234, 345, 456], set[0].Races);

		Assert.Equal("50K v1", set[1].ID);
		Assert.Equal("Test 2", set[1].Name);
		Assert.Equal(10, set[1].HourLimit);
		Assert.Equal([123, 234], set[1].Races);

		Assert.Equal("50K v2", set[2].ID);
		Assert.Equal("Test 3", set[2].Name);
		Assert.Equal(10, set[2].HourLimit);
		Assert.Equal([345, 456], set[2].Races);
	}
}