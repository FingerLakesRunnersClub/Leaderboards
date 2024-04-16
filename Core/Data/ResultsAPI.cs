using System.Net.Http.Json;
using System.Text.Json;

namespace FLRC.Leaderboards.Core.Data;

public sealed class ResultsAPI<T> : IResultsAPI where T : IDataSource
{
	private readonly HttpClient _http;
	public IDataSource Source { get; }

	public ResultsAPI(HttpClient http, T source)
	{
		_http = http;
		Source = source;
	}

	public async Task<JsonElement> GetResults(uint id)
	{
		var url = Source.URL(id);
		return await _http.GetFromJsonAsync<JsonElement>(url);
	}
}