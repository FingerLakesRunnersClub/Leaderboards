using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface ICourseService : IDataService<Course>
{
	Task<Course[]> GetCourses(Guid[] selected);
}