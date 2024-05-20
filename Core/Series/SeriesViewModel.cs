using FLRC.Leaderboards.Core.Ranking;

namespace FLRC.Leaderboards.Core.Series;

public class SeriesViewModel : DataTableViewModel
{
	public override string Title => Series.Name;

	public Series Series { get; init; }
	public RankedList<SeriesResult> Results { get; init; }

	public override string ResponsiveBreakpoint => "xl";
}