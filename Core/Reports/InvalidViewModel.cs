using FLRC.Leaderboards.Core.Races;
using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Reports;

public sealed class InvalidViewModel : DataTableViewModel
{
	public override string Title => "Invalid Results";
	public override string ResponsiveBreakpoint => "lg";
	public IDictionary<Course, Result[]> Results { get; init; }
}