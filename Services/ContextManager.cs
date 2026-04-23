using FLRC.Leaderboards.Core.Config;
using FLRC.Leaderboards.Model;

namespace FLRC.Leaderboards.Services;

public sealed class ContextManager(IContextProvider contextProvider, ISeriesService seriesService) : IContextManager
{
    public async Task<Series> Series()
        => await seriesService.Find(contextProvider.App)
           ?? throw new NullReferenceException($"Series '{contextProvider.App}' not configured!");
}