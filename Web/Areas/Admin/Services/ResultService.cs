using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class ResultService(DB db) : IResultService
{
	private readonly IQueryable<Result> _results
		= db.Set<Result>()
			.Include(r => r.Athlete)
			.AsQueryable();

	public async Task<Result> Get(Guid id)
		=> await _results
			.FirstAsync(r => r.ID == id);

	public async Task<Result[]> Find(Guid courseID)
		=> await _results
			.Where(r => r.CourseID == courseID)
			.ToArrayAsync();

	public async Task Import(Result[] results)
	{
		var newResults = results.Where(newR => !db.Set<Result>().Any(r => r.AthleteID == newR.AthleteID && r.CourseID == newR.CourseID && r.StartTime == newR.StartTime && r.Duration == newR.Duration));
		await db.Set<Result>().AddRangeAsync(newResults);
		await db.SaveChangesAsync();
	}
}