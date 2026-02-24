using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Data;
using FLRC.Leaderboards.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public sealed class AthleteService(DB db) : IAthleteService
{
	private readonly IIncludableQueryable<Athlete, ICollection<LinkedAccount>> _athletes
		= db.Set<Athlete>()
			.Include(a => a.LinkedAccounts);

	public async Task<Athlete> Get(Guid id)
		=> await _athletes.FirstAsync(a => a.ID == id);

	public async Task<Athlete> Find(string link, string value)
		=> await _athletes.FirstOrDefaultAsync(a => a.LinkedAccounts.Any(l => l.Type == link && l.Value == value));

	public async Task<Athlete> Find(string name, DateOnly dob)
		=> await _athletes.FirstOrDefaultAsync(a => a.Name == name && a.DateOfBirth == dob);

	public async Task<Athlete> Find(string name, byte age, DateTime onDate)
		=> await _athletes.FirstOrDefaultAsync(a => a.Name == name && a.DateOfBirth != Athlete.UnknownDOB && (byte)((onDate - new DateTime(a.DateOfBirth.Year, a.DateOfBirth.Month, a.DateOfBirth.Day).ToUniversalTime()).TotalDays / Date.DaysPerYear) == age);

	public async Task AddAthlete(Athlete athlete)
	{
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();
	}

	public static readonly LinkedAccountComparer LinkedAccountComparer = new();

	public async Task UpdateAthlete(Athlete athlete, Athlete updated)
	{
		athlete.Name = updated.Name;
		athlete.Category = updated.Category == Athlete.UnknownCategory ? athlete.Category : updated.Category;
		athlete.DateOfBirth = updated.DateOfBirth == Athlete.UnknownDOB ? athlete.DateOfBirth : updated.DateOfBirth;
		athlete.IsPrivate = updated.IsPrivate || athlete.IsPrivate;

		var newAccounts = updated.LinkedAccounts.Except(athlete.LinkedAccounts, LinkedAccountComparer);
		foreach (var account in newAccounts)
		{
			var newAccount = new LinkedAccount
			{
				ID = Guid.NewGuid(),
				AthleteID = athlete.ID,
				Type = account.Type,
				Value = account.Value
			};
			await db.AddAsync(newAccount);
		}

		await db.SaveChangesAsync();
	}
}