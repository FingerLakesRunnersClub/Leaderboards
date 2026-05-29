using System.Net.Http.Json;
using System.Text.Json;

namespace FLRC.Leaderboards.Core.Data;

public sealed class ResultsAPI<T>(HttpClient http, T source) : IResultsAPI where T : IDataSource
{
	public IDataSource Source { get; } = source;

	public async Task<JsonElement> GetResults(uint id)
	{
		var url = await Source.URL(id);
		return await http.GetFromJsonAsync<JsonElement>(url);
	}
}