using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public sealed class ChallengeService(DB db) : IChallengeService
{
	public Task<Challenge[]> All()
		=> throw new NotImplementedException();

	public Task<Challenge> Get(Guid id)
		=> throw new NotImplementedException();

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

	public Task Update(Challenge challenge, Challenge updated)
		=> throw new NotImplementedException();

	public Task Delete(Challenge challenge)
		=> throw new NotImplementedException();
}