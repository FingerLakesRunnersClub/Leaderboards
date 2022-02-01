using System.Text.Json;

namespace FLRC.Leaderboards.Core;

public interface IDataAPI
{
	Task<JsonElement> GetResults(uint id);
}