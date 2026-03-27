using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Services;

public sealed class IterationService(DB db) : IIterationService
{
	private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

	public async Task<Iteration[]> All()
		=> await db.Set<Iteration>()
			.OrderBy(i => i.Series.Name)
			.ThenByDescending(i => i.StartDate)
			.ToArrayAsync();

	public async Task<Iteration> Get(Guid id)
		=> await db.Set<Iteration>()
			.FirstAsync(i => i.ID == id);

	public async Task<Iteration?> Current(Guid seriesID)
		=> await db.Set<Iteration>()
			.Where(i => i.SeriesID == seriesID && i.StartDate <= Today && i.EndDate >= Today)
			.OrderByDescending(i => i.EndDate)
			.FirstOrDefaultAsync();

	public async Task<Iteration?> MostRecent(Guid seriesID)
		=> await db.Set<Iteration>()
			.Where(i => i.SeriesID == seriesID && i.EndDate < Today)
			.OrderByDescending(i => i.EndDate)
			.FirstOrDefaultAsync();

	public async Task Add(Iteration iteration)
	{
		iteration.ID = Guid.NewGuid();
		await db.AddAsync(iteration);
		await db.SaveChangesAsync();
	}

	public async Task Update(Iteration iteration, Iteration updated)
	{
		iteration.Name = updated.Name;
		iteration.StartDate = updated.StartDate;
		iteration.EndDate = updated.EndDate;

		await db.SaveChangesAsync();
	}

	public async Task UpdateRaces(Iteration iteration, Race[] races)
	{
		iteration.Races.Clear();
		foreach (var race in races)
			iteration.Races.Add(race);

		await db.SaveChangesAsync();
	}

	public async Task UpdateRegistrations(Iteration iteration, Athlete[] athletes)
	{
		iteration.Athletes.Clear();
		foreach (var athlete in athletes)
			iteration.Athletes.Add(athlete);

		await db.SaveChangesAsync();
	}

	public Task Delete(Iteration iteration)
		=> throw new NotImplementedException();
}