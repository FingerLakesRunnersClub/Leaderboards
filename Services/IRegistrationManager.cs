using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IRegistrationManager
{
	Task<Athlete[]> Update(Iteration iteration);
}