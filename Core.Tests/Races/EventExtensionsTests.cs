using FLRC.AgeGradeCalculator;
using FLRC.Leaderboards.Core.Races;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Races;

public sealed class EventExtensionsTests
{
	[Fact]
	public void CanParseTrackEvent()
	{
		//arrange
		const string name = "800m";

		//act
		var trackEvent = name.ToTrackEvent();

		//assert
		Assert.Equal(nameof(TrackEvent._800m), trackEvent);
	}

	[Fact]
	public void CanParseFieldEvent()
	{
		//arrange
		const string name = "Long Jump";

		//act
		var trackEvent = name.ToFieldEvent();

		//assert
		Assert.Equal(nameof(FieldEvent.LongJump), trackEvent);
	}
}