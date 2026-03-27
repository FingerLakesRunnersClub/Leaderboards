using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Services;

public sealed class CourseService(DB db) : ICourseService
{
	public async Task<Course[]> All()
		=> await db.Set<Course>()
			.ToArrayAsync();

	public async Task<Course[]> GetCourses(Guid[] ids)
		=> await db.Set<Course>()
			.Where(c => ids.Any(id => c.ID == id))
			.ToArrayAsync();

	public async Task<Course> Get(Guid id)
		=> await db.Set<Course>()
			.FirstAsync(c => c.ID == id);

	public Task Add(Course course)
		=> throw new NotImplementedException();

	public Task Update(Course course, Course updated)
		=> throw new NotImplementedException();

	public Task Delete(Course course)
		=> throw new NotImplementedException();
}