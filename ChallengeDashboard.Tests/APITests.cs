using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ChallengeDashboard.Tests
{
    public class APITests
    {
        private class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly string _data;

            public MockHttpMessageHandler(string data)
            {
                _data = data;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var content = new StringContent(_data);
                var message = new HttpResponseMessage { Content = content };
                return Task.FromResult(message);
            }
        }

        [Fact]
        public async Task CanGetCourseInfo()
        {
            var data = @"{
                    ""RaceId"": 123,
                    ""Name"": ""Virgil Crest Ultramarathons"",
                    ""SportType"": ""Running (Trail)"",
                    ""Distances"": [{""Name"": ""50K""}],
                    ""Racers"": []
                }";
            var http = new MockHttpMessageHandler(data);
            var configData = new Dictionary<string, string> { { "API", "http://localhost" } };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
            var api = new API(new HttpClient(http), configuration);

            //act
            var course = await api.GetCourse(123);

            //assert
            Assert.Equal((uint)123, course.ID);
            Assert.Equal("Virgil Crest Ultramarathons", course.Name);
            Assert.Equal("Running (Trail)", course.Type);
            Assert.Equal("50K", course.Distance);
        }

        [Fact]
        public async Task CanGetResultsForCourse()
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
            var http = new MockHttpMessageHandler(data);
            var configData = new Dictionary<string, string> { { "API", "http://localhost" } };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
            var api = new API(new HttpClient(http), configuration);

            //act
            var course = await api.GetCourse(123);

            //assert
            var result = course.Results.First();
            Assert.Equal((ushort)234, result.Athlete.ID);
            Assert.Equal("Steve Desmond", result.Athlete.Name);
            Assert.Equal(26, result.Athlete.Age);
            Assert.Equal("M", result.Athlete.Category);
            Assert.Equal(new DateTime(2011, 09, 24), result.StartTime);
            Assert.Equal(new TimeSpan(0, 5, 04, 0), result.Duration);
        }

        [Fact]
        public async Task DNFsAreIgnored()
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
            var http = new MockHttpMessageHandler(data);
            var configData = new Dictionary<string, string> { { "API", "http://localhost" } };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
            var api = new API(new HttpClient(http), configuration);

            //act
            var course = await api.GetCourse(123);

            //assert
            Assert.Empty(course.Results);
        }
    }
}