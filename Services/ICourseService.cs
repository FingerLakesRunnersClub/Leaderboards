using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface ICourseService
{
	Task<Course> Get(Guid id);
}