using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Data;

public interface IDataService
{
	Task<Athlete> GetAthlete(uint id);
	Task<IDictionary<uint, Athlete>> GetAthletes();

	Task<Course> GetResults(uint id, string distance);
	Task<IEnumerable<Course>> GetAllResults();

	Task<IEnumerable<Athlete>> GetGroupMembers(string id);
}