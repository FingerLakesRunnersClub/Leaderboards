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

	[Fact]
	public void CanComparePercentages()
	{
		//arrange
		var p1 = new Percent(76.51);
		var p2 = new Percent(67.89);

		//act
		var c1 = p1.CompareTo(p2);
		var c2 = p2.CompareTo(p1);

		//assert
		Assert.Equal(1, c1);
		Assert.Equal(-1, c2);
	}
}