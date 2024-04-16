using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Races;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Athletes;

public sealed class SimilarAthleteTests
{
	[Fact]
	public void CanGetSimilarity()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };
		var my = new AthleteSummary(CourseData.Athlete1, [course], TestHelpers.Config);
		var their = new AthleteSummary(CourseData.Athlete2, [course], TestHelpers.Config);

		//act
		var similar = new SimilarAthlete(my, their);

		//assert
		Assert.Equal("68%", similar.Similarity.Display);
	}

	[Fact]
	public void CanGetOverlap()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };
		var my = new AthleteSummary(CourseData.Athlete1, [course], TestHelpers.Config);
		var their = new AthleteSummary(CourseData.Athlete2, [course], TestHelpers.Config);

		//act
		var similar = new SimilarAthlete(my, their);

		//assert
		Assert.Equal("50%", similar.Overlap.Display);
	}

	[Fact]
	public void CanCalculateFastestPaceDifference()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };
		var my = new AthleteSummary(CourseData.Athlete1, [course], TestHelpers.Config);
		var their = new AthleteSummary(CourseData.Athlete2, [course], TestHelpers.Config);

		//act
		var similar = new SimilarAthlete(my, their);

		//assert
		Assert.Equal("31.8% faster", similar.FastestPercent.Display);
	}

	[Fact]
	public void CanCalculateScoreForRanking()
	{
		//arrange
		var course = new Course { Results = CourseData.Results, Distance = new Distance("10K") };
		var my = new AthleteSummary(CourseData.Athlete1, [course], TestHelpers.Config);
		var their = new AthleteSummary(CourseData.Athlete2, [course], TestHelpers.Config);

		//act
		var similar = new SimilarAthlete(my, their);

		//assert
		Assert.Equal(68.2 * 0.9 + 68.2 * 0.5 * 0.1, similar.Score, 1);
	}
}