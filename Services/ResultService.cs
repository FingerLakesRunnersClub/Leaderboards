using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Services;

public sealed class ResultService(DB db) : IResultService
{
	public Task<Result[]> All()
		=> throw new NotImplementedException();

	public async Task<Result> Get(Guid id)
		=> await db.Set<Result>()
			.FirstAsync(r => r.ID == id);

	public async Task<Result[]> Find(Guid courseID)
		=> await db.Set<Result>()
			.Where(r => r.CourseID == courseID)
			.ToArrayAsync();

	public async Task Import(Result[] results)
	{
		var newResults = results.Where(newR => !db.Set<Result>().Any(r => r.AthleteID == newR.AthleteID && r.CourseID == newR.CourseID && r.StartTime == newR.StartTime && r.Duration == newR.Duration));
		await db.AddRangeAsync(newResults);
		await db.SaveChangesAsync();
	}

	public Task Add(Result result)
		=> throw new NotImplementedException();

	public Task Update(Result result, Result updated)
		=> throw new NotImplementedException();

	public Task Delete(Result result)
		=> throw new NotImplementedException();
}