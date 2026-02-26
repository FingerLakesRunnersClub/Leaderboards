using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface ICourseService
{
	Task<Course> Get(Guid id);
}