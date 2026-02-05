using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface ISeriesService
{
	Task<Series[]> GetAllSeries();
	Task<Series> GetSeries(Guid id);
	Task AddSeries(Series series, IDictionary<string, bool> features, IDictionary<string, string> settings);
	Task UpdateSeries(Series series, Series updated, IDictionary<string, bool> features, IDictionary<string, string> settings);
	Task<Series> FindSeries(string key);
}