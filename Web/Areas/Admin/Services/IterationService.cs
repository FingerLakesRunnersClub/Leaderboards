using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class IterationService(DB db) : IIterationService
{
	public async Task<Iteration[]> GetAllIterations()
		=> await db.Set<Iteration>()
			.Include(i => i.Series)
			.OrderBy(i => i.Series.Name)
			.ThenByDescending(i => i.StartDate)
			.ToArrayAsync();

	public async Task<Iteration> GetIteration(Guid id)
		=> await db.Set<Iteration>()
			.Include(i => i.Series)
			.Include(i => i.Races)
			.FirstAsync(i => i.ID == id);

	public async Task AddIteration(Guid id, Iteration iteration, Guid[] races)
	{
		iteration.ID = Guid.NewGuid();
		iteration.SeriesID = id;

		await db.Set<Iteration>().AddAsync(iteration);
		await UpdateRaces(iteration, races);

		await db.SaveChangesAsync();
	}

	public async Task UpdateIteration(Iteration iteration, Iteration updated, Guid[] races)
	{
		iteration.Name = updated.Name;
		iteration.StartDate = updated.StartDate;
		iteration.EndDate = updated.EndDate;

		await UpdateRaces(iteration, races);

		await db.SaveChangesAsync();
	}

	private async Task UpdateRaces(Iteration iteration, Guid[] raceIDs)
	{
		var races = await db.Set<Race>().Where(r => raceIDs.Any(id => r.ID == id)).ToArrayAsync();

		iteration.Races.Clear();
		foreach (var race in races)
			iteration.Races.Add(race);
	}
}