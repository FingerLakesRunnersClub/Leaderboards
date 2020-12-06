using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ChallengeDashboard.Tests
{
    public class DataParserTests
    {
        [Fact]
        public void CanGetCourseInfo()
        {
            var data = @"{
                    ""RaceId"": 123,
                    ""Name"": ""Virgil Crest Ultramarathons"",
                    ""SportType"": ""Running (Trail)"",
                    ""Distances"": [{""Name"": ""50K""}],
                    ""Racers"": []
                }";
            var json = JsonDocument.Parse(data).RootElement;

            //act
            var course = DataParser.ParseCourse(json);

            //assert
            Assert.Equal((uint)123, course.ID);
            Assert.Equal("Virgil Crest Ultramarathons", course.Name);
            Assert.Equal("Running (Trail)", course.Type);
            Assert.Equal("50K", course.Distance);
        }

        [Fact]
        public void CanGetResultsForCourse()
        {
            //arrange
            var data = @"{
                    ""RaceId"": 123,
                    ""Name"": ""Virgil Crest Ultramarathons"",
                    ""SportType"": ""Running (Trail)"",
                    ""Distances"": [{""Name"": ""50K""}],
                    ""Racers"": [{
                        ""RacerId"": 234,
                        ""Name"": ""Steve Desmond"",
                        ""Age"": 26,
                        ""Gender"": ""M"",
                        ""StartTime"": ""2011-09-24"",
                        ""Finished"": 1,
                        ""RaceTime"": 18240
                    }]
                }";
            var json = JsonDocument.Parse(data).RootElement;

            //act
            var course = DataParser.ParseCourse(json);
            var result = course.Results.First();

            //assert
            Assert.Equal((ushort)234, result.Athlete.ID);
            Assert.Equal("Steve Desmond", result.Athlete.Name);
            Assert.Equal(26, result.Athlete.Age);
            Assert.Equal("M", result.Athlete.Category);
            Assert.Equal(new DateTime(2011, 09, 24), result.StartTime);
            Assert.Equal(new TimeSpan(0, 5, 04, 0), result.Duration);
        }

        [Fact]
        public void DNFsAreIgnored()
        {
            //arrange
            var data = @"{
                    ""RaceId"": 123,
                    ""Name"": ""Virgil Crest Ultramarathons"",
                    ""SportType"": ""Running (Trail)"",
                    ""Distances"": [{""Name"": ""50K""}],
                    ""Racers"": [{
                        ""RacerId"": 234,
                        ""Name"": ""Steve Desmond"",
                        ""Age"": 26,
                        ""Gender"": ""M"",
                        ""StartTime"": ""2011-09-24"",
                        ""Finished"": 0,
                        ""RaceTime"": 0
                    }]
                }";
            var json = JsonDocument.Parse(data).RootElement;

            //act
            var course = DataParser.ParseCourse(json);

            //assert
            Assert.Empty(course.Results);
        }
    }
}