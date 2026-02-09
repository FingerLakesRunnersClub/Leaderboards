using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class CourseService(DB db) : ICourseService
{
	public async Task<Course> Get(Guid id)
		=> await db.Set<Course>()
			.Include(c => c.Race)
			.Include(c => c.Results)
			.FirstAsync(c => c.ID == id);
}