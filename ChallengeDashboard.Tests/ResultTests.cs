using System;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class ResultTests
    {
        [Fact]
        public void CanGetDisplayTimeOfDuration()
        {
            //arrange
            var result = new Result { Duration = new TimeSpan(0, 1, 2, 3, 456) };

            //act
            var displayTime = result.DisplayTime;

            //assert
            Assert.Equal("1:02:03.4", displayTime);
        }

        [Fact]
        public void CanCompareResultsByDuration()
        {
            //arrange
            var result1 = new Result { Duration = new TimeSpan(2, 3, 4) };

            //act
            var result2 = new Result { Duration = new TimeSpan(1, 2, 3) };

            //assert
            Assert.Equal(1, result1.CompareTo(result2));
            Assert.Equal(-1, result2.CompareTo(result1));
        }
    }
}
