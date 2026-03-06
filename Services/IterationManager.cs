using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public sealed class IterationManager(IContextProvider contextProvider, IIterationService iterationService, ISeriesService seriesService) : IIterationManager
{
	public async Task<Iteration?> ActiveIteration()
	{
		var series = await seriesService.Find(contextProvider.App);

		if (series is null)
			throw new ArgumentNullException($"Series '{contextProvider.App}' not configured!");

		return await iterationService.Current(series.ID)
		       ?? await iterationService.MostRecent(series.ID);
	}
}