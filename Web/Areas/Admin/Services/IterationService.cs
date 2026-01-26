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
			.FirstAsync(i => i.ID == id);

	public async Task AddIteration(Guid id, Iteration iteration)
	{
		iteration.ID = Guid.NewGuid();
		iteration.SeriesID = id;

		await db.Set<Iteration>().AddAsync(iteration);
		await db.SaveChangesAsync();
	}

	public async Task UpdateIteration(Iteration iteration, Iteration updated)
	{
		iteration.Name = updated.Name;
		iteration.StartDate = updated.StartDate;
		iteration.EndDate = updated.EndDate;

		await db.SaveChangesAsync();
	}
}