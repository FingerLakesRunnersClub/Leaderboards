using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IRaceService
{
	Task<Race[]> GetAllRaces();
	Task<Race[]> GetRaces(Guid[] ids);
	Task<Race> GetRace(Guid id);

	Task AddRace(Race race);
	Task UpdateRace(Race race, Race updated);
	Task UpdateCourses(Race race, IDictionary<Guid, Course> courses);
}