using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Overall;
using FLRC.Leaderboards.Core.Races;

namespace FLRC.Leaderboards.Core.Reports;

public sealed class CompletedViewModel : OverallResultsViewModel<Date>
{
	public override string Title => "Completions";
	public override string ResponsiveBreakpoint => "lg";

	public IDictionary<Athlete, DateOnly> PersonalResults { get; init; }
}