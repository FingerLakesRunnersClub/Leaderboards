using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Core.Series;
using FLRC.Leaderboards.Web.Controllers;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public class SeriesControllerTests
{
	[Fact]
	public async Task SelectsCorrectSeries()
	{
		//arrange
		var manager = Substitute.For<ISeriesManager>();
		var settings = new ConfigurationBuilder().AddJsonFile("json/series.json").Build();
		var config = new AppConfig(settings);
		var controller = new SeriesController(manager, config);

		//act
		var result = await controller.Index("100K");

		//assert
		var vm = (SeriesViewModel)result.Model;
		Assert.Equal("Test 1", vm!.Series.Name);
	}
}