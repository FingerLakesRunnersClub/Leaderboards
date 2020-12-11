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
    }
}
