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
            Assert.Equal(new DateTime(2011, 09, 24), result.StartTime);
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
    }
}