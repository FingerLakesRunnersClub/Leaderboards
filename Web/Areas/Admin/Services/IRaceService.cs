using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface IRaceService
{
	Task<Race[]> GetAllRaces();
	Task<Race[]> GetRaces(Guid[] ids);
	Task<Race> GetRace(Guid id);
	Task UpdateRace(Race race, Race updated, IDictionary<Guid, Course> courses);
	Task AddRace(Race race, IDictionary<Guid, Course> courses);
}