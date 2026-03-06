using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface ISeriesService : IDataService<Series>
{
	Task<Series?> Find(string key);

	Task UpdateSettings(Series series, IDictionary<string, string> settings);
	Task UpdateFeatures(Series series, IDictionary<string, bool> features);
}