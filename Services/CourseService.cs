using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Services;

public sealed class CourseService(DB db) : ICourseService
{
	private readonly IQueryable<Course> _courseDetails
		= db.Set<Course>()
		.Include(c => c.Race)
		.Include(c => c.Results)
		.AsQueryable();

	public async Task<Course> Get(Guid id)
		=> await _courseDetails
			.FirstAsync(c => c.ID == id);
}