using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Services;

public sealed class SeriesService(DB db) : ISeriesService
{
	public async Task<Series[]> All()
		=> await db.Set<Series>()
			.OrderBy(s => s.Key)
			.ToArrayAsync();

	public async Task<Series> Get(Guid id)
		=> await db.Set<Series>()
			.Include(s => s.Features)
			.Include(s => s.Settings)
			.FirstAsync(s => s.ID == id);

	public async Task<Series?> Find(string key)
		=> await db.Set<Series>()
			.Include(s => s.Features)
			.Include(s => s.Settings)
			.FirstAsync(s => s.Key == key);

	public async Task Add(Series series)
	{
		series.ID = Guid.NewGuid();
		await db.AddAsync(series);
		await db.SaveChangesAsync();
	}

	public async Task Update(Series series, Series updated)
	{
		series.Key = updated.Key;
		series.Name = updated.Name;
		await db.SaveChangesAsync();
	}

	public async Task UpdateSettings(Series series, IDictionary<string, string> settings)
	{
		series.Settings.Clear();
		foreach (var setting in settings)
			series.Settings.Add(new Setting { SeriesID = series.ID, Key = setting.Key, Value = setting.Value });
		await db.SaveChangesAsync();
	}

	public async Task UpdateFeatures(Series series, IDictionary<string, bool> features)
	{
		series.Features.Clear();
		foreach (var feature in features)
			series.Features.Add(new Feature { SeriesID = series.ID, Key = feature.Key, Value = feature.Value });
		await db.SaveChangesAsync();
	}

	public Task Delete(Series series)
		=> throw new NotImplementedException();
}