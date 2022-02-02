using FLRC.Leaderboards.Core.Results;

namespace FLRC.Leaderboards.Core.Reports;

public class InvalidViewModel : DataTableViewModel
{
	public override string Title => "Invalid Results";
	public override string ResponsiveBreakpoint => "lg";
	public IDictionary<Course, IEnumerable<Result>> Results { get; init; }
}