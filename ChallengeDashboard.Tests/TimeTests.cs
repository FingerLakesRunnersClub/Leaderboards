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
            var time = new Time(TimeSpan.Parse("00:01:23"));

            //act
            var display = time.Display;

            //assert
            Assert.Equal("1:23", display);
        }

        [Fact]
        public void TimeDisplaysHoursForLongerSpans()
        {
            //arrange
            var time = new Time(TimeSpan.Parse("01:02:03"));

            //act
            var display = time.Display;

            //assert
            Assert.Equal("1:02:03", display);
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

        [Fact]
        public void CanEquateTimes()
        {
            //arrange
            var t1 = new Time(new TimeSpan(1, 2, 3));
            var t2 = new Time(new TimeSpan(1, 2, 3));

            //act
            var equal = t1.Equals(t2);
            
            //assert
            Assert.True(equal);
        }

        [Fact]
        public void CanEquateToObject()
        {
            //arrange
            var t1 = new Time(new TimeSpan(1, 2, 3));
            var t2 = new Time(new TimeSpan(1, 2, 3));

            //act
            var equal = t1.Equals((object)t2);
            
            //assert
            Assert.True(equal);
        }

        [Fact]
        public void CanGetHashCode()
        {
            //arrange
            var t1 = new Time(new TimeSpan(1, 2, 3));
            var t2 = new Time(new TimeSpan(1, 2, 3));

            //act
            var h1 = t1.GetHashCode();
            var h2 = t1.GetHashCode();
            
            //assert
            Assert.Equal(h1, h2);
        }
    }
}