using System.Text;
using FLRC.Leaderboards.Core.Series;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Series;

public class SeriesSetTests
{
	[Fact]
	public void CanCreateSeriesSetFromConfig()
	{
		//arrange
		const string json = @"{
			""Series"": [
				{
					""ID"": ""100K"",
					""Name"": ""Test"",
					""HourLimit"": 10,
					""Races"": [123, 234]
				},
				{
					""ID"": ""200K"",
					""Name"": ""Test"",
					""HourLimit"": 20,
					""Races"": [123, 234, 345]
				}
			]
		}";

		var data = new MemoryStream(Encoding.UTF8.GetBytes(json));
		var config = new ConfigurationBuilder().AddJsonStream(data).Build();

		//act
		var set = new SeriesSet(config.GetSection("Series"));

		//assert
		Assert.Equal(2, set.Count);

		Assert.Equal("100K", set[0].ID);
		Assert.Equal(10, set[0].HourLimit);
		Assert.Equal([123, 234], set[0].Races);

		Assert.Equal("200K", set["200K"].ID);
		Assert.Equal(20, set["200K"].HourLimit);
		Assert.Equal([123, 234, 345], set["200K"].Races);
	}
}