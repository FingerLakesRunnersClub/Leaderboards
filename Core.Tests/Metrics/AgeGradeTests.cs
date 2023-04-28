using FLRC.Leaderboards.Core.Metrics;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Metrics;

public sealed class AgeGradeTests
{
	[Fact]
	public void CanDisplayRoundedAgeGradeAsPercent()
	{
		//arrange
		var ageGrade = new AgeGrade(98.765);

		//act
		var display = ageGrade.Display;

		//assert
		Assert.Equal("98.77%", display);
	}

	[Fact]
	public void CanCompareAgeGrades()
	{
		//arrange
		var ag1 = new AgeGrade(75);

		//act
		var ag2 = new AgeGrade(80);

		//assert
		Assert.Equal(-1, ag1.CompareTo(ag2));
		Assert.Equal(1, ag2.CompareTo(ag1));
	}
}