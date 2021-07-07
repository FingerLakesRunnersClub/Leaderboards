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
        public async Task CanParseAthlete()
        {
            //arrange
            var data = await File.ReadAllTextAsync("json/athlete.json");
            var json = JsonDocument.Parse(data).RootElement;
            var element = json.GetProperty("Racers").EnumerateArray().First();
            
            //act
            var athlete = DataParser.ParseAthlete(element);

            //assert
            Assert.Equal((ushort)234, athlete.ID);
            Assert.Equal("Steve Desmond", athlete.Name);
            Assert.Equal(26, athlete.Age);
            Assert.Equal(AgeGradeCalculator.Category.M, athlete.Category.Value);
            Assert.Equal(new DateTime(1985, 02, 16), athlete.DateOfBirth);
        }

        [Fact]
        public void CanParseDuration()
        {
            //arrange
            const double seconds = 123.12;
            
            //act
            var duration = DataParser.ParseDuration(seconds);
            
            //assert
            Assert.Equal(new TimeSpan(0, 2, 4), duration.Value);
        }

        [Fact]
        public void DurationRoundsAppropriately()
        {
            //arrange
            const double seconds = 123.09;
            
            //act
            var duration = DataParser.ParseDuration(seconds);
            
            //assert
            Assert.Equal(new TimeSpan(0, 2, 3), duration.Value);
        }

        [Fact]
        public async Task CanGetResultsForCourse()
        {
            //arrange
            var data = await File.ReadAllTextAsync("json/athlete.json");
            var json = JsonDocument.Parse(data).RootElement;
            var course = new Course();

            //act
            var results = DataParser.ParseCourse(course, json);
            var result = results.First();

            //assert
            Assert.Equal((ushort)234, result.Athlete.ID);
            Assert.Equal("Steve Desmond", result.Athlete.Name);
            Assert.Equal(26, result.Athlete.Age);
            Assert.Equal(new DateTime(1985, 02, 16), result.Athlete.DateOfBirth);
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
            var course = new Course();

            //act
            var results = DataParser.ParseCourse(course, json);

            //assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task FalseStartsAreIgnored()
        {
            //arrange
            var data = await File.ReadAllTextAsync("json/false-start.json");
            var json = JsonDocument.Parse(data).RootElement;
            var course = new Course();

            //act
            var results = DataParser.ParseCourse(course, json);

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