using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class AthleteSummaryTests
    {
        [Fact]
        public void CanGetTotalNumberOfResults()
        {
            //arrange
            var course1 = new Course {Results = CourseData.Results};
            var course2 = new Course {Results = CourseData.Results};
            var course3 = new Course {Results = CourseData.Results};
            var results = new[] {course1, course2, course3};

            //act
            var summary = new AthleteSummary(CourseData.Athlete1, results);

            //assert
            Assert.Equal(6, summary.TotalResults);
        }
    }
}