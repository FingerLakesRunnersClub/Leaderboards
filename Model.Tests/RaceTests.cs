using Xunit;

namespace FLRC.Leaderboards.Model.Tests;

public sealed class RaceTests
{
	[Fact]
	public void CanGetDistanceDisplay()
	{
		//arrange
		var race = new Race
		{
			Courses = [
				new Course { Distance = 5, Units = "km"},
				new Course { Distance = 10, Units = "mi"}
			]
		};

		//act
		var display = race.DistanceDisplay;

		//assert
		Assert.Equal("5 km, 10 mi", display);
	}
}