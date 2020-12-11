using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class GroupedResultTests
    {
        [Fact]
        public void CanGetAverageDuration()
        {
            var athlete = new Athlete();

            //arrange
            var results = new List<Result>
            {
                new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:00") },
                new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:10") },
                new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:20") },
                new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:30") }
            };

            var groupedResult = new GroupedResult(results.GroupBy(r => r.Athlete).First());

            //act
            var avg = groupedResult.Average();

            //assert
            Assert.Equal(TimeSpan.Parse("1:15"), avg.Duration);
        }
        
        [Fact]
        public void CanGetAverageFromTopAttempts()
        {
            var athlete = new Athlete();

            //arrange
            var results = new List<Result>
            {
                new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:00") },
                new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:10") },
                new Result { Athlete = athlete, Duration = TimeSpan.Parse("1:20") }
            };

            var groupedResult = new GroupedResult(results.GroupBy(r => r.Athlete).First());

            //act
            var avg = groupedResult.Average(2);

            //assert
            Assert.Equal(TimeSpan.Parse("1:05"), avg.Duration);
        }
    }
}