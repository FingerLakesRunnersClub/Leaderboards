using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace FLRC.Leaderboards.Core.Data;

public class DataAPI : IDataAPI
{
	private readonly HttpClient _httpClient;
	private readonly string _baseURL;

	public DataAPI(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_baseURL = configuration.GetValue<string>("API");
	}

	public async Task<JsonElement> GetResults(uint id)
	{
		var url = $"{_baseURL}/results?raceid={id}";
		var response = await _httpClient.GetStreamAsync(url);
		var json = await JsonDocument.ParseAsync(response);
		return json.RootElement;
	}
}