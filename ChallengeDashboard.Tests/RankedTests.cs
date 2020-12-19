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
            var rankedResult = new Ranked<Result> { BehindLeader = new Time(TimeSpan.Parse("00:01:23.4")) };

            //act
            var behind = rankedResult.BehindLeader.Display;

            //assert
            Assert.Equal("1:23.4", behind);
        }

        [Fact]
        public void CanDisplayRoundedAgeGradeAsPercent()
        {
            //arrange
            var rankedResult = new Ranked<Result> { AgeGrade = new AgeGrade(98.76) };

            //act
            var ageGrade = rankedResult.AgeGrade.Display;

            //assert
            Assert.Equal("98.8%", ageGrade);
        }
    }
}