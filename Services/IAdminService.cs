using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IAdminService : IDataService<Admin>
{
	Task<bool> Verify(Guid id);
}