using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public sealed class IterationManager(IContextManager contextManager, IIterationService iterationService) : IIterationManager
{
	public async Task<Iteration?> ActiveIteration()
	{
		var series = await contextManager.Series();

		return await iterationService.Current(series.ID)
		       ?? await iterationService.MostRecent(series.ID);
	}
}