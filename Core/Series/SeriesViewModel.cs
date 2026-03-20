using FLRC.Leaderboards.Core.Ranking;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Series;

public sealed class SeriesViewModel : DataTableViewModel
{
	public override string Title => Series.Name;

	public Series Series { get; init; }
	public RankedList<SeriesResult, Result> Results { get; init; }

	public override string ResponsiveBreakpoint => "xl";
}