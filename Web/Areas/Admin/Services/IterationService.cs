using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class IterationService(DB db) : IIterationService
{
	private readonly IQueryable<Iteration> _iterations
		= db.Set<Iteration>()
			.Include(i => i.Series)
			.Include(i => i.Races)
			.ThenInclude(r => r.Courses)
			.Include(i => i.Athletes.OrderBy(a => a.Name))
			.ThenInclude(a => a.LinkedAccounts.OrderBy(l => l.Type))
			.AsQueryable();

	public async Task<Iteration[]> GetAllIterations()
		=> await _iterations
			.OrderBy(i => i.Series.Name)
			.ThenByDescending(i => i.StartDate)
			.ToArrayAsync();

	public async Task<Iteration> GetIteration(Guid id)
		=> await _iterations
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
}