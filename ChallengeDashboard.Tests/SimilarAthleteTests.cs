using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class SimilarAthleteTests
    {
        [Fact]
        public void CanGetSimilarity()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };
            var my = new AthleteSummary(CourseData.Athlete1, new[] {course});
            var their = new AthleteSummary(CourseData.Athlete2, new[] {course});

            //act
            var similar = new SimilarAthlete(my, their);

            //assert
            Assert.Equal("68%", similar.Similarity.Display);
        }
        
        [Fact]
        public void CanGetConfidence()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };
            var my = new AthleteSummary(CourseData.Athlete1, new[] {course});
            var their = new AthleteSummary(CourseData.Athlete2, new[] {course});

            //act
            var similar = new SimilarAthlete(my, their);

            //assert
            Assert.Equal("50%", similar.Confidence.Display);
        }
        
        [Fact]
        public void CanCalculateFastestPaceDifference()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };
            var my = new AthleteSummary(CourseData.Athlete1, new[] {course});
            var their = new AthleteSummary(CourseData.Athlete2, new[] {course});

            //act
            var similar = new SimilarAthlete(my, their);

            //assert
            Assert.Equal("31.8% faster", similar.FastestPercent.Display);
        }
        
        [Fact]
        public void CanCalculateScoreForRanking()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };
            var my = new AthleteSummary(CourseData.Athlete1, new[] {course});
            var their = new AthleteSummary(CourseData.Athlete2, new[] {course});

            //act
            var similar = new SimilarAthlete(my, their);

            //assert
            Assert.Equal((68.2 * 0.9) + (68.2 * 0.5 * 0.1), similar.Score, 1);
        }
    }
}