using System.Collections.Generic;

namespace FLRC.ChallengeDashboard;

public class InvalidViewModel : DataTableViewModel
{
	public override string Title => "Invalid Results";
	public override string ResponsiveBreakpoint => "lg";
	public IDictionary<Course, IEnumerable<Result>> Results { get; init; }
}
