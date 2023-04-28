using System.Net.Http.Json;
using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Core.Groups;

public sealed class GroupAPI : IGroupAPI
{
	private readonly HttpClient _httpClient;
	private readonly string _url;

	public GroupAPI(HttpClient httpClient, IConfig config)
	{
		_httpClient = httpClient;
		_url = config.GroupAPI;
	}

	public async Task<IDictionary<string, IReadOnlyCollection<uint>>> GetGroups()
		=> await _httpClient.GetFromJsonAsync<IDictionary<string, IReadOnlyCollection<uint>>>(_url);
}