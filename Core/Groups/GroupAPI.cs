using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Groups;

public class GroupAPI : IGroupAPI
{
	private readonly HttpClient _httpClient;
	private readonly string _url;

	public GroupAPI(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_url = configuration.GetValue<string>("GroupAPI");
	}

	public async Task<IDictionary<string, IEnumerable<uint>>> GetGroups()
	{
		var response = await _httpClient.GetStreamAsync(_url);
		return await JsonSerializer.DeserializeAsync<IDictionary<string, IEnumerable<uint>>>(response);
	}
}