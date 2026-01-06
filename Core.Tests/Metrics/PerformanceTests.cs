using FLRC.Leaderboards.Core.Metrics;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Metrics;

public sealed class PerformanceTests
{
	[Fact]
	public void ConvertsToMetersCorrectly()
	{
		//arrange
		var performance = new Performance("3'3.37\"");

		//act
		var meters = performance.Meters;

		//assert
		Assert.Equal(1, meters, 5);
	}
}