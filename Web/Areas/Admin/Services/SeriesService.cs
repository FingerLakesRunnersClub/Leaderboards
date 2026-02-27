using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class SeriesService(DB db) : ISeriesService
{
	private readonly IQueryable<Series> _series
		= db.Set<Series>()
			.Include(s => s.Features)
			.Include(s => s.Settings)
			.AsQueryable();

	public async Task<Series[]> GetAllSeries()
		=> await db.Set<Series>().OrderBy(s => s.Key).ToArrayAsync();

	public async Task<Series> GetSeries(Guid id)
		=> await _series.FirstAsync(s => s.ID == id);

	public async Task<Series> FindSeries(string key)
		=> await _series.FirstAsync(s => s.Key == key);

	public async Task AddSeries(Series series, IDictionary<string, bool> features, IDictionary<string, string> settings)
	{
		series.ID = Guid.NewGuid();

		UpdateFeatures(series, features);
		UpdateSettings(series, settings);

		await db.AddAsync(series);
		await db.SaveChangesAsync();
	}

	public async Task UpdateSeries(Series series, Series updated, IDictionary<string, bool> features, IDictionary<string, string> settings)
	{
		series.Key = updated.Key;
		series.Name = updated.Name;

		UpdateFeatures(series, features);
		UpdateSettings(series, settings);

		await db.SaveChangesAsync();
	}

	private static void UpdateSettings(Series series, IDictionary<string, string> settings)
	{
		series.Settings.Clear();
		foreach (var setting in settings)
			series.Settings.Add(new Setting { SeriesID = series.ID, Key = setting.Key, Value = setting.Value });
	}

	private static void UpdateFeatures(Series series, IDictionary<string, bool> features)
	{
		series.Features.Clear();
		foreach (var feature in features)
			series.Features.Add(new Feature { SeriesID = series.ID, Key = feature.Key, Value = feature.Value });
	}
}