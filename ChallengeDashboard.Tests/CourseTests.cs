using System;
using System.Collections.Generic;
using Xunit;

namespace ChallengeDashboard.Tests
{
    public class CourseTests
    {
        private static readonly Athlete a1 = new Athlete { Category = "M" };
        private static readonly Athlete a2 = new Athlete { Category = "F" };
        private static readonly Athlete a3 = new Athlete { Category = "M" };
        private static readonly Athlete a4 = new Athlete { Category = "F" };

        private static readonly IEnumerable<Result> results = new List<Result>()
        {
            new Result { Athlete = a1, Duration = TimeSpan.Parse("1:23:45.6") },
            new Result { Athlete = a1, Duration = TimeSpan.Parse("2:34:56.7") },
            new Result { Athlete = a2, Duration = TimeSpan.Parse("0:54:32.1") },
            new Result { Athlete = a3, Duration = TimeSpan.Parse("1:02:03.4") },
            new Result { Athlete = a3, Duration = TimeSpan.Parse("1:00:00.0") },
            new Result { Athlete = a4, Duration = TimeSpan.Parse("2:03:04.5") },
            new Result { Athlete = a4, Duration = TimeSpan.Parse("2:22:22.2") },
            new Result { Athlete = a4, Duration = TimeSpan.Parse("2:00:00.0") }
        };

        [Fact]
        public void CanGetFastestResults()
        {
            //arrange
            var course = new Course { Results = results };

            //act
            var fastest = course.Fastest().ToArray();

            //assert
            Assert.Equal(4, fastest.Length);
            Assert.Equal(a2, fastest[0].Athlete);
            Assert.Equal(a3, fastest[1].Athlete);
            Assert.Equal(a1, fastest[2].Athlete);
            Assert.Equal(a4, fastest[3].Athlete);
        }

        [Fact]
        public void CanGetFastestResultsForCategory()
        {
            //arrange
            var course = new Course { Results = results };

            //act
            var fastest = course.Fastest(Category.F).ToArray();

            //assert
            Assert.Equal(2, fastest.Length);
            Assert.Equal(a2, fastest[0].Athlete);
            Assert.Equal(a4, fastest[1].Athlete);
        }

        [Fact]
        public void CanGetMostRuns()
        {
            //arrange
            var course = new Course { Results = results };

            //act
            var most = course.MostRuns().ToArray();

            //assert
            Assert.Equal(4, most.Length);
            Assert.Equal(a4, most[0].Athlete);
            Assert.Equal(a3, most[1].Athlete);
            Assert.Equal(a1, most[2].Athlete);
            Assert.Equal(a2, most[3].Athlete);
        }

        [Fact]
        public void CanGetMostRunsForCategory()
        {
            //arrange
            var course = new Course { Results = results };

            //act
            var most = course.MostRuns(Category.M).ToArray();

            //assert
            Assert.Equal(2, most.Length);
            Assert.Equal(a3, most[0].Athlete);
            Assert.Equal(a1, most[1].Athlete);
        }

        [Fact]
        public void CanGetBestAverage()
        {
            //arrange
            var course = new Course { Results = results };

            //act
            var avg = course.BestAverage().ToArray();

            //assert
            Assert.Equal(3, avg.Length);
            Assert.Equal(a3, avg[0].Athlete);
            Assert.Equal(a1, avg[1].Athlete);
            Assert.Equal(a4, avg[2].Athlete);
        }

        [Fact]
        public void CanGetBestAverageForCategory()
        {
            //arrange
            var course = new Course { Results = results };

            //act
            var avg = course.BestAverage(Category.M).ToArray();

            //assert
            Assert.Equal(2, avg.Length);
            Assert.Equal(a3, avg[0].Athlete);
            Assert.Equal(a1, avg[1].Athlete);
        }

        [Fact]
        public void BestAverageDropsAthletesBelowThreshold()
        {
            //arrange
            var course = new Course { Results = results };

            //act
            var avg = course.BestAverage(Category.F).ToArray();

            //assert
            Assert.Single(avg);
            Assert.Equal(a4, avg[0].Athlete);
        }
    }
}