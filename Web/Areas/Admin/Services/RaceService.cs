using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class RaceService(DB db) : IRaceService
{
	private readonly IQueryable<Race> _races
		= db.Set<Race>()
			.Include(r => r.Courses.OrderBy(c => c.Distance).ThenBy(c => c.Units))
			.OrderBy(r => r.Name)
			.AsQueryable();

	public async Task<Race[]> GetAllRaces()
		=> await _races.ToArrayAsync();

	public async Task<Race[]> GetRaces(Guid[] ids)
		=> await _races
			.Where(r => ids.Any(id => r.ID == id))
			.ToArrayAsync();

	public async Task<Race> GetRace(Guid id)
		=> await _races
			.FirstAsync(r => r.ID == id);

	public async Task AddRace(Race race, IDictionary<Guid, Course> courses)
	{
		race.ID = Guid.NewGuid();
		await db.AddAsync(race);

		await UpdateCourses(race, courses);

		await db.SaveChangesAsync();
	}

	public async Task UpdateRace(Race race, Race updated, IDictionary<Guid, Course> courses)
	{
		race.Name = updated.Name;
		race.Type = updated.Type;

		await UpdateCourses(race, courses);

		await db.SaveChangesAsync();
	}

	private async Task UpdateCourses(Race race, IDictionary<Guid, Course> courses)
	{
		foreach (var course in courses)
		{
			var existing = race.Courses.FirstOrDefault(c => c.ID == course.Key);
			if (existing is not null)
			{
				existing.Distance = course.Value.Distance;
				existing.Units = course.Value.Units;
				db.Update(existing);
			}
			else if (course.Value.Distance > 0 && !string.IsNullOrWhiteSpace(course.Value.Units))
			{
				var newCourse = new Course
				{
					ID = Guid.NewGuid(),
					RaceID = race.ID,
					Distance = course.Value.Distance,
					Units = course.Value.Units
				};
				await db.AddAsync(newCourse);
			}
		}
	}
}