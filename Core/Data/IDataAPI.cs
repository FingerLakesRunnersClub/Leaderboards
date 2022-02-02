using System.Text.Json;

namespace FLRC.Leaderboards.Core.Data;

public interface IDataAPI
{
	Task<JsonElement> GetResults(uint id);
}