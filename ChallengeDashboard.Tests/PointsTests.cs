using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class PointsTests
{
	[Fact]
	public void CanDisplayRoundedPoints()
	{
		//arrange
		var points = new Points(98.765);

		//act
		var display = points.Display;

		//assert
		Assert.Equal("98.77", display);
	}

	[Fact]
	public void CanComparePoints()
	{
		//arrange
		var p1 = new Points(12.34);

		//act
		var p2 = new Points(23.45);

		//assert
		Assert.Equal(1, p2.CompareTo(p1));
		Assert.Equal(-1, p1.CompareTo(p2));
	}
}