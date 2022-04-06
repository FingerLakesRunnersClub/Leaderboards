using System.Text.Json;

namespace FLRC.Leaderboards.Core.Data;

public class ResultsAPI<T> : IResultsAPI where T : IDataSource
{
	private readonly HttpClient _httpClient;
	public IDataSource Source { get; }

	public ResultsAPI(HttpClient httpClient, T source)
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