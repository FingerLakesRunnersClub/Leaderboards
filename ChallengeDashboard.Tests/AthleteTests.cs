using System;
using FLRC.AgeGradeCalculator;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class AthleteTests
    {
        [Theory]
        [InlineData(Category.F, "F")]
        [InlineData(Category.M, "M")]
        [InlineData(null, null)]
        public void CanDisplayCategory(Category? category, string expected)
        {
            //arrange
            var athlete = new Athlete { Category = category };

            //act
            var display = athlete.CategoryDisplay;

            //assert
            Assert.Equal(expected, display);
        }

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

        [Theory]
        [InlineData(20, 2)]
        [InlineData(29, 2)]
        [InlineData(70, 7)]
        [InlineData(79, 7)]
        public void CanGetTeamForAge(byte age, byte expected)
        {
            //arrange
            var athlete = new Athlete { Age = age };

            //act
            var team = athlete.Team;

            //assert
            Assert.Equal(expected, team);
        }

        [Fact]
        public void CanGetTeamDisplayNameFromAge()
        {
            //arrange
            var athlete = new Athlete { Age = 25 };

            //act
            var teamName = athlete.TeamDisplay;

            //assert
            Assert.StartsWith("20", teamName);
            Assert.EndsWith("29", teamName);
        }
    }
}