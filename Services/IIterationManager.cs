using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public interface IIterationManager
{
	Task<Iteration?> ActiveIteration();
}