using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Data;

public interface IDataService
{
	Task<Athlete> GetAthlete(uint id);
	Task<IDictionary<uint, Athlete>> GetAthletes();

	Task<Course> GetResults(uint id, string distance);
	Task<IReadOnlyCollection<Course>> GetAllResults();

	Task<IReadOnlyCollection<Athlete>> GetGroupMembers(string id);
	Task<IDictionary<Athlete, DateOnly>> GetPersonalCompletions();

	Task<IReadOnlyCollection<User>> GetCommunityUsers();
	Task<IReadOnlyCollection<User>> GetCommunityGroupMembers(string groupID);
	Task AddCommunityGroupMembers(IDictionary<string, string[]> groupAdditions);
}