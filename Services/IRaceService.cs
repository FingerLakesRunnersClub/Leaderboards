using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IRaceService : IDataService<Race>
{
	Task<Race[]> GetRaces(Guid[] ids);

	Task UpdateCourses(Race race, IDictionary<Guid, Course> courses);
}