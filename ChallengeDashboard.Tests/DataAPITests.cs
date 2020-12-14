using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class DataAPITests
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
        public async Task CanGetJSONFromResponse()
        {
            var data = await File.ReadAllTextAsync("json/empty.json");
            var http = new MockHttpMessageHandler(data);
            var configData = new Dictionary<string, string> { { "API", "http://localhost" } };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
            var api = new DataAPI(new HttpClient(http), configuration);

            //act
            var json = await api.GetCourse(123);

            //assert
            Assert.Equal((uint)123, json.GetProperty("RaceId").GetUInt32());
            Assert.Equal("Virgil Crest Ultramarathons", json.GetProperty("Name").GetString());
            Assert.Equal("Running (Trail)", json.GetProperty("SportType").GetString());
        }
    }
}