using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Services;

public sealed class ResultService(DB db) : IResultService
{
	public async Task<Result[]> All()
		=> await db.Set<Result>().ToArrayAsync();

	public async Task<Result> Get(Guid id)
		=> await db.Set<Result>()
			.FirstAsync(r => r.ID == id);

	public async Task<Result[]> Find(Guid courseID)
		=> await db.Set<Result>()
			.Include(r => r.Athlete)
			.Where(r => r.CourseID == courseID)
			.ToArrayAsync();

	public async Task<Result[]> Find(Iteration iteration)
		=> await db.Set<Result>()
			.Include(r => r.Athlete)
			.Where(r => iteration.Races.Contains(r.Course.Race)
				&& (iteration.StartDate == null || r.StartTime >= iteration.StartDate.Value.ToDateTime(TimeOnly.MinValue))
				&& (iteration.EndDate == null || r.StartTime <= iteration.EndDate.Value.ToDateTime(TimeOnly.MaxValue)))
			.ToArrayAsync();

	public async Task Import(Result[] results)
	{
		var newResults = results.Where(newR => !db.Set<Result>().Any(r => r.AthleteID == newR.AthleteID && r.CourseID == newR.CourseID && r.StartTime == newR.StartTime && r.Duration == newR.Duration));
		await db.AddRangeAsync(newResults);
		await db.SaveChangesAsync();
	}

	public async Task Add(Result result)
	{
		await db.AddAsync(result);
		await db.SaveChangesAsync();
	}

	public async Task Update(Result result, Result updated)
	{
		result.StartTime = updated.StartTime;
		result.Duration = updated.Duration;
		await db.SaveChangesAsync();
	}

	public async Task Delete(Result result)
	{
		db.Remove(result);
		await db.SaveChangesAsync();
	}
}