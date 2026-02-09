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

	public async Task AddAthlete(Athlete athlete)
	{
		await db.AddAsync(athlete);
		await db.SaveChangesAsync();
	}
}