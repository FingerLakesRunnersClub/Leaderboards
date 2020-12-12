using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FLRC.AgeGradeCalculator;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class DataParserTests
    {
        [Fact]
        public async Task CanGetCourseInfo()
        {
            var data = await File.ReadAllTextAsync("json/empty.json");
            var json = JsonDocument.Parse(data).RootElement;

            //act
            var course = DataParser.ParseCourse(json);

            //assert
            Assert.Equal((uint)123, course.ID);
            Assert.Equal("Virgil Crest Ultramarathons", course.Name);
            Assert.Equal("Running (Trail)", course.Type);
            Assert.Equal(50000, course.Distance);
        }

        [Fact]
        public async Task CanGetResultsForCourse()
        {
            //arrange
            var data = await File.ReadAllTextAsync("json/athlete.json");
            var json = JsonDocument.Parse(data).RootElement;

            //act
            var course = DataParser.ParseCourse(json);
            var result = course.Results.First();

            //assert
            Assert.Equal((ushort)234, result.Athlete.ID);
            Assert.Equal("Steve Desmond", result.Athlete.Name);
            Assert.Equal(26, result.Athlete.Age);
            Assert.Equal(Category.M, result.Athlete.Category);
            Assert.Equal(new DateTime(2011, 09, 24), result.StartTime);
            Assert.Equal(new TimeSpan(0, 5, 04, 0), result.Duration);
        }

        [Fact]
        public async Task DNFsAreIgnored()
        {
            //arrange
            var data = await File.ReadAllTextAsync("json/dnf.json");
            var json = JsonDocument.Parse(data).RootElement;

            //act
            var course = DataParser.ParseCourse(json);

            //assert
            Assert.Empty(course.Results);
        }
    }
}