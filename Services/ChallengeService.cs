using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Services;

public sealed class ChallengeService(DB db) : IChallengeService
{
	private readonly IQueryable<Challenge> _challenges
		= db.Set<Challenge>()
			.Include(c => c.Iteration)
			.OrderByDescending(c => c.Iteration.Name)
			.ThenByDescending(c => c.IsOfficial)
			.ThenByDescending(c => c.IsPrimary)
			.ThenBy(c => c.Name)
			.AsQueryable();

	private readonly IQueryable<Challenge> _challengeDetails
		= db.Set<Challenge>()
			.Include(c => c.Courses).ThenInclude(c => c.Race)
			.Include(c => c.Iteration).ThenInclude(i => i.Races).ThenInclude(r => r.Courses)
			.AsQueryable();

	public async Task<Challenge[]> All()
		=> await _challenges.ToArrayAsync();

	public async Task<Challenge> Get(Guid id)
		=> await _challengeDetails
			.FirstAsync(c => c.ID == id);

	public async Task Add(Challenge challenge)
	{
		challenge.ID = Guid.NewGuid();
		await db.AddAsync(challenge);
		await db.SaveChangesAsync();
	}

	public async Task AddConnection(Athlete athlete, Challenge challenge)
	{
		athlete.Challenges.Add(challenge);
		await db.SaveChangesAsync();
	}

	public async Task Update(Challenge challenge, Challenge updated)
	{
		challenge.Name = updated.Name;
		await db.SaveChangesAsync();
	}

	public async Task UpdateCourses(Challenge challenge, Course[] courses)
	{
		var toAdd = courses.Except(challenge.Courses).ToArray();
		foreach (var course in toAdd)
			challenge.Courses.Add(course);

		var toDelete = challenge.Courses.Except(courses).ToArray();
		foreach (var course in toDelete)
			challenge.Courses.Remove(course);

		await db.SaveChangesAsync();
	}

	public Task Delete(Challenge challenge)
		=> throw new NotImplementedException();
}