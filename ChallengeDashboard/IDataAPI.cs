using System.Text.Json;

namespace FLRC.ChallengeDashboard;

public interface IDataAPI
{
	Task<JsonElement> GetResults(uint id);
}