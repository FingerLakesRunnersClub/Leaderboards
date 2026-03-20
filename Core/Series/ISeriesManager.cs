using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Series;

public interface ISeriesManager
{
	Task<IDictionary<Series, RankedList<SeriesResult, Result>>> Fastest();
	Task<IDictionary<Series, RankedList<SeriesResult, Result>>> Earliest();
}