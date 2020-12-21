using System;
using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class CourseTests
    {
        [Fact]
        public void CanGetFastestResults()
        {
            //arrange
            var course = new Course { Results = CourseData.Results };

            //act
            var fastest = course.Fastest().ToArray();

            //assert
            Assert.Equal(4, fastest.Length);
            Assert.Equal(CourseData.Athlete2, fastest[0].Athlete);
            Assert.Equal(CourseData.Athlete3, fastest[1].Athlete);
            Assert.Equal(CourseData.Athlete1, fastest[2].Athlete);
            Assert.Equal(CourseData.Athlete4, fastest[3].Athlete);
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
            Assert.Equal(CourseData.Athlete2, fastest[0].Athlete);
            Assert.Equal(CourseData.Athlete4, fastest[1].Athlete);
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
            Assert.Equal(CourseData.Athlete4, most[0].Athlete);
            Assert.Equal(CourseData.Athlete3, most[1].Athlete);
            Assert.Equal(CourseData.Athlete1, most[2].Athlete);
            Assert.Equal(CourseData.Athlete2, most[3].Athlete);
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
            Assert.Equal(CourseData.Athlete3, most[0].Athlete);
            Assert.Equal(CourseData.Athlete1, most[1].Athlete);
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
            Assert.Equal(CourseData.Athlete3, avg[0].Athlete);
            Assert.Equal(CourseData.Athlete1, avg[1].Athlete);
            Assert.Equal(CourseData.Athlete4, avg[2].Athlete);
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
            Assert.Equal(CourseData.Athlete3, avg[0].Athlete);
            Assert.Equal(CourseData.Athlete1, avg[1].Athlete);
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
            Assert.Equal(CourseData.Athlete4, avg[0].Athlete);
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
    }
}