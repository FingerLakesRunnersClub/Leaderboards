using System;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class TimeTests
    {
        [Fact]
        public void CanGetTimeToDisplay()
        {
            //arrange
            var time = new Time(TimeSpan.Parse("00:01:23.4"));

            //act
            var display = time.Display;

            //assert
            Assert.Equal("1:23.4", display);
        }

        [Fact]
        public void TimeDisplaysHoursForLongerSpans()
        {
            //arrange
            var time = new Time(TimeSpan.Parse("01:02:03.4"));

            //act
            var display = time.Display;

            //assert
            Assert.Equal("1:02:03.4", display);
        }
    }
}