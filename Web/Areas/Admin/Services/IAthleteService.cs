using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface IAthleteService
{
	Task<Athlete> Get(Guid id);

	Task<Athlete> Find(string link, string value);
	Task<Athlete> Find(string name, DateOnly dob);
	Task<Athlete> Find(string legacyAthleteName, byte legacyAthleteAge, DateTime date);

	Task AddAthlete(Athlete athlete);
	Task UpdateAthlete(Athlete athlete, Athlete updated);
}