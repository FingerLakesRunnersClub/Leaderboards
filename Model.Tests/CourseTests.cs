using Xunit;

namespace FLRC.Leaderboards.Model.Tests;

public sealed class CourseTests
{
	[Fact]
	public void CanGetDistanceDisplay()
	{
		//arrange
		var course = new Course { Distance = 5, Units = "km"};

		//act
		var display = course.DistanceDisplay;

		//assert
		Assert.Equal("5 km", display);
	}
}