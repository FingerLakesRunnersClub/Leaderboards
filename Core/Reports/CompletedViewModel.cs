using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Reports;

public class CompletedViewModel : OverallResultsViewModel<Date>
{
	public override string Title => "Completions";
	public override string ResponsiveBreakpoint => "lg";
}