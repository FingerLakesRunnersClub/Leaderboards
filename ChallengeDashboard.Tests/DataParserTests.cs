using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class DataParserTests
    {
        [Fact]
        public async Task CanGetResultsForCourse()
        {
            //arrange
            var data = await File.ReadAllTextAsync("json/athlete.json");
            var json = JsonDocument.Parse(data).RootElement;

            //act
            var results = DataParser.ParseCourse(json);
            var result = results.First();

            //assert
            Assert.Equal((ushort)234, result.Athlete.ID);
            Assert.Equal("Steve Desmond", result.Athlete.Name);
            Assert.Equal(26, result.Athlete.Age);
            Assert.Equal(AgeGradeCalculator.Category.M, result.Athlete.Category.Value);
            Assert.Equal(new DateTime(2011, 09, 24), result.StartTime.Value);
            Assert.Equal(new Time(new TimeSpan(0, 5, 04, 0)), result.Duration);
        }

        [Fact]
        public async Task DNFsAreIgnored()
        {
            //arrange
            var data = await File.ReadAllTextAsync("json/dnf.json");
            var json = JsonDocument.Parse(data).RootElement;

            //act
            var results = DataParser.ParseCourse(json);

            //assert
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("1 mile", Course.MetersPerMile)]
        [InlineData("2 miles", 2 * Course.MetersPerMile)]
        [InlineData("3 mi", 3 * Course.MetersPerMile)]
        [InlineData("1000 m", 1000)]
        [InlineData("2 km", 2000)]
        [InlineData("3 K", 3000)]
        public void CanParseDistance(string distance, double expected)
        {
            //act
            var actual = DataParser.ParseDistance(distance);

            //assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("F", AgeGradeCalculator.Category.F)]
        [InlineData("M", AgeGradeCalculator.Category.M)]
        [InlineData("X", null)]
        public void CanParseCategory(string cat, AgeGradeCalculator.Category? expected)
        {
            //act
            var category = DataParser.ParseCategory(cat);

            //assert
            if (expected != null)
                Assert.Equal(expected, category.Value);
            else
                Assert.Null(category);
        }
    }
}