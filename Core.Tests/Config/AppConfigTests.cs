using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Config;

public sealed class AppConfigTests
{
	[Fact]
	public void CanGetCourseNames()
	{
		//arrange
		var config = TestHelpers.Config;

		//act
		var courseNames = config.CourseNames;

		//assert
		Assert.Equal("Virgil Crest Ultramarathons", courseNames.First().Value);
	}
}