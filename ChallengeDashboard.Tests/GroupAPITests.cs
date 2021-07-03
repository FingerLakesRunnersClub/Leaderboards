using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests
{
    public class GroupAPITests
    {
        [Fact]
        public async Task CanGetGroupMemberIDs()
        {
            //arrange
            const string data = @"{ ""Test 1"": [ 123, 234 ], ""Test 2"": [ 234, 345 ] }";
            var http = new MockHttpMessageHandler(data);
            var configData = new Dictionary<string, string> { { "GroupAPI", "http://localhost" } };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
            var api = new GroupAPI(new HttpClient(http), configuration);
            
            //act
            var json = await api.GetGroups();
            
            //assert
            var groups = json.EnumerateObject().ToArray();
            Assert.Equal("Test 1", groups[0].Name);
            Assert.Equal("Test 2", groups[1].Name);

            var members1 = groups[0].Value.EnumerateArray().Select(v => v.GetUInt16()).ToArray();
            Assert.Equal(123, members1[0]);
            Assert.Equal(234, members1[1]);

            var members2 = groups[1].Value.EnumerateArray().Select(v => v.GetUInt16()).ToArray();
            Assert.Equal(234, members2[0]);
            Assert.Equal(345, members2[1]);
        }
    }
}