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

        [Fact]
        public void CanSubtractTimes()
        {
            //arrange
            var t1 = new Time(new TimeSpan(1, 2, 3));
            var t2 = new Time(new TimeSpan(4, 6, 8));

            //act
            var diff = t2.Subtract(t1);

            //assert
            Assert.Equal(new TimeSpan(3, 4, 5), diff.Value);
        }

        [Fact]
        public void CanCompareTimes()
        {
            //arrange
            var t1 = new Time(new TimeSpan(1, 2, 3));

            //act
            var t2 = new Time(new TimeSpan(2, 1, 0));

            //assert
            Assert.Equal(-1, t1.CompareTo(t2));
            Assert.Equal(1, t2.CompareTo(t1));
        }
    }
}