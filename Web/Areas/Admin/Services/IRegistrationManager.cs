using FLRC.Leaderboards.Data.Models;

namespace FLRC.Leaderboards.Web.Areas.Admin.Services;

public interface IRegistrationManager
{
	Task<Athlete[]> Update(Iteration iteration);
}