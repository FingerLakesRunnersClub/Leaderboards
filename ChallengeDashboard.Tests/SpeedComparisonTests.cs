using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class SpeedComparisonTests
{
	[Theory]
	[InlineData(-5.43, "5.4% faster")]
	[InlineData(3.56, "3.6% slower")]
	[InlineData(0, "Same")]
	public void CanGetSpeedComparisonFromPercentDifference(double value, string expected)
	{
		//arrange
		var comparison = new SpeedComparison(value);

		//act
		var display = comparison.Display;

		//assert
		Assert.Equal(expected, display);
	}
}