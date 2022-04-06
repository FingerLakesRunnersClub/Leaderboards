using System.Text.Json;

namespace FLRC.Leaderboards.Core.Data;

public interface IResultsAPI
{
	IDataSource Source { get; }
	Task<JsonElement> GetResults(uint id);
}