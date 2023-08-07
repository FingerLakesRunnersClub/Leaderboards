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

	public async Task<IDictionary<string, IReadOnlyCollection<uint>>> GetGroups()
		=> await _httpClient.GetFromJsonAsync<IDictionary<string, IReadOnlyCollection<uint>>>(_config.GroupAPI);
}