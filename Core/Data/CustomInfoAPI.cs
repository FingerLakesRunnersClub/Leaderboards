using System.Collections.Immutable;
using System.Net.Http.Json;
using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Core.Data;

public sealed class CustomInfoAPI : ICustomInfoAPI
{
	private readonly HttpClient _httpClient;
	private readonly IConfig _config;

	public CustomInfoAPI(HttpClient httpClient, IConfig config)
	{
		_httpClient = httpClient;
		_config = config;
	}

	public async Task<IDictionary<string, string>> GetAliases()
		=> !string.IsNullOrWhiteSpace(_config.AliasAPI)
			? await _httpClient.GetFromJsonAsync<IDictionary<string, string>>(_config.AliasAPI)
			: ImmutableDictionary<string, string>.Empty;

	public async Task<IDictionary<string, uint[]>> GetGroups()
		=> !string.IsNullOrWhiteSpace(_config.GroupAPI)
			? await _httpClient.GetFromJsonAsync<IDictionary<string, uint[]>>(_config.GroupAPI)
			: ImmutableDictionary<string, uint[]>.Empty;

	public async Task<IDictionary<uint, DateOnly>> GetPersonalCompletions()
		=> !string.IsNullOrWhiteSpace(_config.PersonalAPI)
			? await _httpClient.GetFromJsonAsync<IDictionary<uint, DateOnly>>(_config.PersonalAPI)
			: ImmutableDictionary<uint, DateOnly>.Empty;
}