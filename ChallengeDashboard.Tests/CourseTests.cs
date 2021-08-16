using System;
using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class CourseTests
    {
        [Fact]
        public void CanGetMilesForCourse()
        {
            //arrange
            var course = new Course { Meters = 3 * Course.MetersPerMile };

            //act
            var miles = course.Miles;

            //assert
            Assert.Equal(3, miles);
        }

        [Fact]
        public void CanGetFastestResults()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var fastest = course.Fastest().ToArray();

            //assert
            Assert.Equal(4, fastest.Length);
            Assert.Equal(CourseData.Athlete2, fastest[0].Result.Athlete);
            Assert.Equal(CourseData.Athlete3, fastest[1].Result.Athlete);
            Assert.Equal(CourseData.Athlete1, fastest[2].Result.Athlete);
            Assert.Equal(CourseData.Athlete4, fastest[3].Result.Athlete);
        }

        [Fact]
        public void CanGetFastestResultsForCategory()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var fastest = course.Fastest(Category.F).ToArray();

            //assert
            Assert.Equal(2, fastest.Length);
            Assert.Equal(CourseData.Athlete2, fastest[0].Result.Athlete);
            Assert.Equal(CourseData.Athlete4, fastest[1].Result.Athlete);
        }

        [Fact]
        public void CanGetMostRuns()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var most = course.MostRuns().ToArray();

            //assert
            Assert.Equal(4, most.Length);
            Assert.Equal(CourseData.Athlete4, most[0].Result.Athlete);
            Assert.Equal(CourseData.Athlete3, most[1].Result.Athlete);
            Assert.Equal(CourseData.Athlete1, most[2].Result.Athlete);
            Assert.Equal(CourseData.Athlete2, most[3].Result.Athlete);
        }

        [Fact]
        public void CanGetMostRunsForCategory()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var most = course.MostRuns(Category.M).ToArray();

            //assert
            Assert.Equal(2, most.Length);
            Assert.Equal(CourseData.Athlete3, most[0].Result.Athlete);
            Assert.Equal(CourseData.Athlete1, most[1].Result.Athlete);
        }

        [Fact]
        public void CanGetMostMiles()
        {
            //arrange
            var course = new Course { Results = CourseData.Results, Meters = 2 * Course.MetersPerMile };

            //act
            var most = course.MostMiles().ToArray();

            //assert
            Assert.Equal(4, most.Length);
            Assert.Equal(CourseData.Athlete4, most[0].Result.Athlete);
            Assert.Equal(CourseData.Athlete3, most[1].Result.Athlete);
            Assert.Equal(CourseData.Athlete1, most[2].Result.Athlete);
            Assert.Equal(CourseData.Athlete2, most[3].Result.Athlete);
        }

        [Fact]
        public void CanGetMostMilesForCategory()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var most = course.MostMiles(Category.M).ToArray();

            //assert
            Assert.Equal(2, most.Length);
            Assert.Equal(CourseData.Athlete3, most[0].Result.Athlete);
            Assert.Equal(CourseData.Athlete1, most[1].Result.Athlete);
        }

        [Fact]
        public void CanGetBestAverage()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var avg = course.BestAverage().ToArray();

            //assert
            Assert.Equal(3, avg.Length);
            Assert.Equal(CourseData.Athlete3, avg[0].Result.Athlete);
            Assert.Equal(CourseData.Athlete1, avg[1].Result.Athlete);
            Assert.Equal(CourseData.Athlete4, avg[2].Result.Athlete);
        }

        [Fact]
        public void CanGetBestAverageForCategory()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var avg = course.BestAverage(Category.M).ToArray();

            //assert
            Assert.Equal(2, avg.Length);
            Assert.Equal(CourseData.Athlete3, avg[0].Result.Athlete);
            Assert.Equal(CourseData.Athlete1, avg[1].Result.Athlete);
        }

        [Fact]
        public void BestAverageDropsAthletesBelowThreshold()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var avg = course.BestAverage(Category.F).ToArray();

            //assert
            Assert.Single(avg);
            Assert.Equal(CourseData.Athlete4, avg[0].Result.Athlete);
        }

        [Fact]
        public void CanGetPointsForFastestTime()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var results = course.Fastest();

            //assert
            Assert.Equal(100, results.First().Points.Value);
        }

        [Fact]
        public void CanGetPointsBasedOnFastestTime()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var results = course.Fastest(Category.M);

            //assert
            Assert.Equal(75, results.Skip(1).First().Points.Value);
        }

        [Fact]
        public void AverageAgeGradeBasedOnIndividualFastestTime()
        {
            //arrange
            var course = new Course { Results = CourseData.Results, Meters = 10000 };

            //act
            var results = course.TeamPoints();

            //assert
            Assert.Equal("43.81%", results.First().AverageAgeGrade.Display);
        }

        [Fact]
        public void CanGetStatisticsForCourse()
        {
            //arrange
            var course = new Course { Results = CourseData.Results, Meters = 10000 };

            //act
            var stats = course.Statistics();

            //assert
            Assert.Equal(4, stats.Participants[string.Empty]);
            Assert.Equal(8, stats.Runs[string.Empty]);
            Assert.Equal(8 * 10000 / Course.MetersPerMile, stats.Miles[string.Empty]);
            Assert.Equal(2, stats.Average[string.Empty]);
        }

        [Fact]
        public void CanGetResultsAsOfDate()
        {
            //arrange
            var course = new Course { Results = CourseData.Results, Meters = 10000 };

            //act
            var results = course.ResultsAsOf(new DateTime(2020, 1, 5));

            //assert
            Assert.Equal(5, results.Count());
        }

        [Fact]
        public void RanksWithTiesSkipCorrectly()
        {
            //arrange
            var courseResults = CourseData.Results.ToList();
            courseResults.Add(new Result
            {
                Course = CourseData.Course,
                Athlete = CourseData.Athlete1,
                StartTime = new Date(DateTime.Parse("1/9/2020")),
                Duration = new Time(TimeSpan.Parse("0:54:32.1"))
            });
            var course = new Course { Results = courseResults, Meters = 10000 };

            //act
            var results = course.Fastest();

            //assert
            Assert.Equal(1, results[0].Rank.Value);
            Assert.Equal(1, results[1].Rank.Value);
            Assert.Equal(3, results[2].Rank.Value);
        }
    }
}