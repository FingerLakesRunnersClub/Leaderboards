﻿using Xunit;

namespace FLRC.Leaderboards.Core.Tests;

public sealed class ConfigTests
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