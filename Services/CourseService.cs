using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Services;

public sealed class CourseService(DB db) : ICourseService
{
	private readonly IQueryable<Course> _courses
		= db.Set<Course>()
			.Include(c => c.Race)
			.AsQueryable();

	private readonly IQueryable<Course> _courseDetails
		= db.Set<Course>()
			.Include(c => c.Race)
			.Include(c => c.Results)
			.AsQueryable();

	public async Task<Course[]> All()
		=> await _courses
			.ToArrayAsync();

	public async Task<Course[]> GetCourses(Guid[] ids)
		=> await _courses
			.Where(c => ids.Any(id => c.ID == id))
			.ToArrayAsync();

	public async Task<Course> Get(Guid id)
		=> await _courseDetails
			.FirstAsync(c => c.ID == id);

	public Task Add(Course course)
		=> throw new NotImplementedException();

	public Task Update(Course course, Course updated)
		=> throw new NotImplementedException();

	public Task Delete(Course course)
		=> throw new NotImplementedException();
}