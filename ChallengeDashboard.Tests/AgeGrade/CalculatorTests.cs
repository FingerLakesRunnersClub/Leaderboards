using System;
using System.Threading.Tasks;
using FLRC.ChallengeDashboard.AgeGrade;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests.AgeGrade
{
    public class CalculatorTests
    {
        [Fact]
        public async Task CanGetAgeGradeForWorldRecordAtStandardDistance()
        {
            //arrange
            var calculator = new AgeGradeCalculator(await Loader.Load());
            var athlete = new Athlete { Category = Category.M, Age = 18 };
            var course = new Course { Distance = 1609.344 };

            //act
            var ageGrade = calculator.GetAgeGrade(athlete, course, TimeSpan.Parse("0:03:47"));

            //assert
            Assert.Equal(100, ageGrade);
        }

        [Fact]
        public async Task CanGetAgeGradeForStandardDistanceAtMaxFactor()
        {
            //arrange
            var calculator = new AgeGradeCalculator(await Loader.Load());
            var athlete = new Athlete { Category = Category.M, Age = 20 };
            var course = new Course { Distance = 1609.344 };

            //act
            var ageGrade = calculator.GetAgeGrade(athlete, course, TimeSpan.Parse("0:04:15"));

            //assert
            Assert.Equal(89.0, ageGrade, 1);
        }

        [Fact]
        public async Task CanGetAgeGradeForStandardDistance()
        {
            //arrange
            var calculator = new AgeGradeCalculator(await Loader.Load());
            var athlete = new Athlete { Category = Category.M, Age = 40 };
            var course = new Course { Distance = 1609.344 };

            //act
            var ageGrade = calculator.GetAgeGrade(athlete, course, TimeSpan.Parse("0:04:30"));

            //assert
            Assert.Equal(88.5, ageGrade, 1);
        }

        [Fact]
        public async Task CanGetInterpolatedAgeGrade()
        {
            //arrange
            var calculator = new AgeGradeCalculator(await Loader.Load());
            var athlete = new Athlete { Category = Category.F, Age = 50 };
            var course = new Course { Distance = 3000 };

            //act
            var ageGrade = calculator.GetAgeGrade(athlete, course, TimeSpan.Parse("0:10:00"));

            //assert
            Assert.Equal(95.4, ageGrade, 1);
        }
    }
}