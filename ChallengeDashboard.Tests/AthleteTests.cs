using System;
using Xunit;

namespace ChallengeDashboard.Tests
{
    public class AthleteTests
    {
        [Theory]
        [InlineData(2000, 01, 01, 21)]
        [InlineData(2000, 01, 02, 20)]
        [InlineData(1950, 01, 01, 71)]
        [InlineData(1950, 01, 02, 70)]
        public void CanGetAgeAsOfACertainDate(ushort year, byte month, byte day, byte expected)
        {
            //arrange
            var athlete = new Athlete { DateOfBirth = new DateTime(year, month, day) };

            var asOf = new DateTime(2021, 1, 1);

            //act
            var age = athlete.AgeAsOf(asOf);

            //assert
            Assert.Equal(expected, age);
        }
    }
}