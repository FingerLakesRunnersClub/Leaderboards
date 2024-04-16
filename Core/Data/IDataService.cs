using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Community;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Data;

public interface IDataService
{
	Task<Athlete> GetAthlete(uint id);
	Task<IDictionary<uint, Athlete>> GetAthletes();

	Task<Course> GetResults(uint id, string distance);
	Task<Course[]> GetAllResults();

	Task<Athlete[]> GetGroupMembers(string id);
	Task<IDictionary<Athlete, DateOnly>> GetPersonalCompletions();

	Task<User[]> GetCommunityUsers();
	Task<User[]> GetCommunityGroupMembers(string groupID);
	Task AddCommunityGroupMembers(IDictionary<string, string[]> groupAdditions);
}