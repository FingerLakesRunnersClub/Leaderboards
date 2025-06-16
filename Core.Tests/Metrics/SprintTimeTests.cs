using FLRC.Leaderboards.Core.Metrics;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Metrics;

public sealed class SprintTimeTests
{
	[Fact]
	public void CanGetTimeToDisplay()
	{
		//arrange
		var time = new SprintTime(TimeSpan.Parse("00:00:01.23"));

		//act
		var display = time.Display;

		//assert
		Assert.Equal("1.23", display);
	}

	[Fact]
	public void TimeDisplaysMinutesForLongerSpans()
	{
		//arrange
		var time = new SprintTime(TimeSpan.Parse("00:01:02.03"));

		//act
		var display = time.Display;

		//assert
		Assert.Equal("1:02.03", display);
	}
}