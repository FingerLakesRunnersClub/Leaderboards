using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class ResultService(DB db) : IResultService
{
	public async Task<Result> Get(Guid id)
		=> await db.Set<Result>().FindAsync(id);

	public async Task<Result[]> Find(Guid courseID)
		=> await db.Set<Result>()
			.Where(r => r.CourseID == courseID)
			.ToArrayAsync();

	public async Task Import(Result[] results)
	{
		await db.Set<Result>().AddRangeAsync(results);
		await db.SaveChangesAsync();
	}
}