using System.Text.Json;

namespace FLRC.Leaderboards.Core.Data;

public class DataAPI<T> : IDataAPI where T : IDataSource
{
	private readonly HttpClient _httpClient;
	public IDataSource Source { get; }

	public DataAPI(HttpClient httpClient, T source)
	{
		_httpClient = httpClient;
		Source = source;
	}

	public async Task<JsonElement> GetResults(uint id)
	{
		var url = Source.URL(id);
		var response = await _httpClient.GetStreamAsync(url);
		var json = await JsonDocument.ParseAsync(response);
		return json.RootElement;
	}
}