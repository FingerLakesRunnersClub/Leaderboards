using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Model;
using Microsoft.EntityFrameworkCore;

namespace FLRC.Leaderboards.Services;

public sealed class AthleteService(DB db) : IAthleteService
{
	private readonly IQueryable<Athlete> _athletes
		= db.Set<Athlete>()
			.Include(a => a.Admins)
			.Include(a => a.LinkedAccounts)
			.Include(a => a.Registrations)
			.Include(a => a.Results)
			.AsQueryable();

	private readonly IQueryable<Athlete> _athleteDetails
		= db.Set<Athlete>()
			.Include(a => a.Admins)
			.Include(a => a.LinkedAccounts.OrderBy(l => l.Type).ThenBy(l => l.Value))
			.Include(a => a.Registrations).ThenInclude(i => i.Series)
			.Include(a => a.Results)
			.Include(a => a.Challenges).ThenInclude(c => c.Iteration)
			.AsQueryable();

	public async Task<Athlete[]> All()
		=> await _athletes
			.OrderBy(a => a.Name)
			.ToArrayAsync();

	public async Task<Athlete> Get(Guid id)
		=> await _athleteDetails
			.FirstAsync(a => a.ID == id);

	public async Task<Athlete?> Find(string link, string value)
		=> await _athleteDetails
			.FirstOrDefaultAsync(a => a.LinkedAccounts.Any(l => l.Type == link && l.Value == value));

	public async Task<Athlete?> Find(string name, DateOnly dob)
		=> await _athleteDetails
			.FirstOrDefaultAsync(a => a.Name == name && a.DateOfBirth == dob);

	public async Task<Athlete?> Find(string name, byte age, DateTime onDate)
		=> await _athleteDetails
			.Where(a => a.Name == name).ToAsyncEnumerable()
			.FirstOrDefaultAsync(a => a.DateOfBirth is not null && a.AgeAsOf(onDate) == age);

	public async Task Add(Athlete athlete)
	{
		athlete.ID = Guid.NewGuid();
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();
	}

	public async Task Update(Athlete athlete, Athlete updated)
	{
		athlete.Name = updated.Name;
		athlete.Category = updated.Category == Athlete.UnknownCategory ? athlete.Category : updated.Category;
		athlete.DateOfBirth = updated.DateOfBirth ?? athlete.DateOfBirth;
		athlete.IsPrivate = updated.IsPrivate || athlete.IsPrivate;

		var newAccounts = updated.LinkedAccounts.Except(athlete.LinkedAccounts, LinkedAccount.Comparer);
		foreach (var account in newAccounts)
		{
			await AddLinkedAccount(athlete, account);
		}

		await db.SaveChangesAsync();
	}

	public async Task AddLinkedAccount(Athlete athlete, LinkedAccount account)
	{
		var newAccount = new LinkedAccount
		{
			ID = Guid.NewGuid(),
			AthleteID = athlete.ID,
			Type = account.Type,
			Value = account.Value
		};
		await db.AddAsync(newAccount);
		await db.SaveChangesAsync();
	}

	public async Task AddAdmin(Athlete athlete)
	{
		var admin = new Admin { ID = athlete.ID };
		await db.AddAsync(admin);
		await db.SaveChangesAsync();
	}

	public async Task RemoveAdmin(Athlete athlete)
	{
		athlete.Admins.Clear();
		await db.SaveChangesAsync();
	}

	public async Task MigrateLinkedAccounts(Athlete from, Athlete to)
	{
		var newAccounts = from.LinkedAccounts.Except(to.LinkedAccounts, LinkedAccount.Comparer).ToArray();
		foreach (var account in newAccounts)
			to.LinkedAccounts.Add(account);
		from.LinkedAccounts.Clear();

		await db.SaveChangesAsync();
	}

	public async Task MigrateRegistrations(Athlete from, Athlete to)
	{
		var newRegistrations = from.Registrations.ToArray();
		foreach (var registration in newRegistrations)
			to.Registrations.Add(registration);
		from.Registrations.Clear();

		await db.SaveChangesAsync();
	}

	public async Task MigrateResults(Athlete from, Athlete to)
	{
		var newResults = from.Results.ToArray();
		foreach (var result in newResults)
			to.Results.Add(result);
		from.Results.Clear();

		await db.SaveChangesAsync();
	}

	public async Task Delete(Athlete athlete)
	{
		db.Remove(athlete);
		await db.SaveChangesAsync();
	}
}