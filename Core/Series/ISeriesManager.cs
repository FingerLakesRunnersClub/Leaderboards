using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Series;

public interface ISeriesManager
{
	Task<IDictionary<Series, RankedList<SeriesResult>>> Fastest();
	Task<IDictionary<Series, RankedList<SeriesResult>>> Earliest();
}