using FLRC.Leaderboards.Core.Races;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Races;

public sealed class CourseTests
{
	[Fact]
	public void ShortNameIsDistanceWhenSet()
	{
		//arrange
		var course = new Course { Race = new Race { Name = "Sprinty" }, Distance = new Distance("60m") };

		//act
		var shortName = course.ShortName;

		//assert
		Assert.Equal("60m", shortName);
	}

	[Fact]
	public void ShortNameIsRaceNameWhenDistanceNotSet()
	{
		//arrange
		var course = new Course { Race = new Race { Name = "Sprinty" } };

		//act
		var shortName = course.ShortName;

		//assert
		Assert.Equal("Sprinty", shortName);
	}
}