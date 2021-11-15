using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FLRC.ChallengeDashboard.Tests;

public class DataAPITests
{
	[Fact]
	public async Task CanGetJSONFromResponse()
	{
		var data = await File.ReadAllTextAsync("json/empty.json");
		var http = new MockHttpMessageHandler(data);
		var configData = new Dictionary<string, string> { { "API", "http://localhost" } };
		var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();
		var api = new DataAPI(new HttpClient(http), configuration);

		//act
		var json = await api.GetResults(123);

		//assert
		Assert.Equal((uint)123, json.GetProperty("RaceId").GetUInt32());
		Assert.Equal("Virgil Crest Ultramarathons", json.GetProperty("Name").GetString());
		Assert.Equal("Running (Trail)", json.GetProperty("SportType").GetString());
	}
}
