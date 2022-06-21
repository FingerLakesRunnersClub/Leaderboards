using FLRC.Leaderboards.Core.Races;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Races;

public class DistanceTests
{
	[Theory]
	[InlineData("1 mile", Distance.MetersPerMile)]
	[InlineData("2 miles", 2 * Distance.MetersPerMile)]
	[InlineData("3 mi", 3 * Distance.MetersPerMile)]
	[InlineData("1000 m", 1000)]
	[InlineData("2 km", 2000)]
	[InlineData("3 K", 3000)]
	[InlineData("marathon", Distance.MetersPerMarathon)]
	[InlineData("half marathon", Distance.MetersPerMarathon / 2)]
	public void CanCalculateMeters(string distance, double expected)
	{
		//act
		var actual = new Distance(distance);

		//assert
		Assert.Equal(expected, actual.Meters);
	}

	[Fact]
	public void CanGetMiles()
	{
		//arrange
		var distance = new Distance("3 miles");

		//act
		var miles = distance.Miles;

		//assert
		Assert.Equal(3, miles);
	}

	[Fact]
	public void CanCompareDistances()
	{
		//arrange
		var d1 = new Distance("1km");
		var d2 = new Distance("2km");
		var d3 = new Distance("1000m");

		//act
		var c1 = d1.CompareTo(d2);
		var c2 = d2.CompareTo(d3);
		var c3 = d1.CompareTo(d3);

		//assert
		Assert.Equal(-1, c1);
		Assert.Equal(1, c2);
		Assert.Equal(0, c3);
	}
}