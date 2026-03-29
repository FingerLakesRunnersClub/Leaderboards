using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IAthleteService : IDataService<Athlete>
{
	Task<Athlete?> Find(string link, string value);
	Task<Athlete?> Find(string name, DateOnly dob);
	Task<Athlete?> Find(string legacyAthleteName, byte legacyAthleteAge, DateTime date);

	Task AddLinkedAccount(Athlete athlete, LinkedAccount account);

	Task MigrateResults(Athlete from, Athlete to);
	Task MigrateRegistrations(Athlete from, Athlete to);
	Task MigrateLinkedAccounts(Athlete from, Athlete to);
}