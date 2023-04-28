using System.Collections.Immutable;
using System.Net.Http.Json;
using FLRC.Leaderboards.Core.Config;

namespace FLRC.Leaderboards.Core.Athletes;

public sealed class AliasAPI : IAliasAPI
{
	private readonly HttpClient _httpClient;
	private readonly string _url;

	public AliasAPI(HttpClient httpClient, IConfig config)
	{
		_httpClient = httpClient;
		_url = config.AliasAPI;
	}

	public async Task<IDictionary<string, string>> GetAliases()
		=> !string.IsNullOrWhiteSpace(_url)
			? await _httpClient.GetFromJsonAsync<IDictionary<string, string>>(_url)
			: ImmutableDictionary<string, string>.Empty;
}