using Xunit;

namespace FLRC.Leaderboards.Web.Tests;

public sealed class OverallExtensionsTests
{
	[Fact]
	public void CanCalculateAverageAgeGrade()
	{
		//arrange
		var group = ResultsData.Results.Fastest().GroupBy(r => r.Result.Athlete);

		//act
		var avgAgeGrade = group.First().AvgAgeGrade();

		//assert
		Assert.Equal(52.84, avgAgeGrade, 0.001);
	}
}