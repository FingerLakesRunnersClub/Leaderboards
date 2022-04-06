using FLRC.Leaderboards.Core.Data;
using Xunit;

namespace FLRC.Leaderboards.Core.Tests.Data;

public class ResultsAPITests
{
	[Fact]
	public async Task CanGetJSONFromResponse()
	{
		var data = await File.ReadAllTextAsync("json/empty.json");
		var http = new MockHttpMessageHandler(data);
		var configData = new Dictionary<string, string> { { "API", "http://localhost" } };
		var api = new ResultsAPI<WebScorer>(new HttpClient(http), new WebScorer(TestHelpers.Config));

		//act
		var json = await api.GetResults(123);

		//assert
		Assert.Equal((uint)123, json.GetProperty("RaceId").GetUInt32());
		Assert.Equal("Virgil Crest Ultramarathons", json.GetProperty("Name").GetString());
		Assert.Equal("Running (Trail)", json.GetProperty("SportType").GetString());
	}
}