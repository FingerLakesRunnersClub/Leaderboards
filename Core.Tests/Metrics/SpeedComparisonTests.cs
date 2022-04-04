using FLRC.Leaderboards.Core.Metrics;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Metrics;

public class SpeedComparisonTests
{
	[Theory]
	[InlineData(-5.43, "5.4% faster")]
	[InlineData(3.56, "3.6% slower")]
	[InlineData(0, "Same")]
	[InlineData(0.09, "Same")]
	[InlineData(-0.08, "Same")]
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