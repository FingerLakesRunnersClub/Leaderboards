using System.Text.Json;

namespace FLRC.Leaderboards.Core.Data;

public interface IDataAPI
{
	IDataSource Source { get; }
	Task<JsonElement> GetResults(uint id);
}