using System;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class RankedTests
    {
        [Fact]
        public void CanGetTimeBehindToDisplay()
        {
            //arrange
            var rankedResult = new Ranked<Result> { BehindLeader = TimeSpan.Parse("00:01:23.4")};

            //act
            var behind = rankedResult.BehindLeaderDisplay;

            //assert
            Assert.Equal("+0:01:23.4", behind);
        }

        [Fact]
        public void CanDisplayRoundedAgeGradeAsPercent()
        {
            //arrange
            var rankedResult = new Ranked<Result> { AgeGrade = 98.76 };

            //act
            var ageGrade = rankedResult.AgeGradeDisplay;

            //assert
            Assert.Equal("98.8%", ageGrade);
        }
    }
}