using FLRC.Leaderboards.Core.Metrics;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Metrics;

public sealed class PercentTests
{
	[Fact]
	public void CanGetDisplayedValueFromDecimal()
	{
		//arrange
		var percent = new Percent(12.3);

		//act
		var display = percent.Display;

		//assert
		Assert.Equal("12%", display);
	}

	[Fact]
	public void DisplayedValueIsRounded()
	{
		//arrange
		var percent = new Percent(76.51);

		//act
		var display = percent.Display;

		//assert
		Assert.Equal("77%", display);
	}
}