using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;

namespace FLRC.Leaderboards.Core.Results;

public sealed class AthleteFieldEventResultsViewModel : CourseResultsViewModel<Performance>
{
	public override string Title => $"{Athlete.Name} — {Course.Name}";
	public Athlete Athlete => Header.Athlete;

	public AthleteHeader Header { get; init; }

	public override string ResponsiveBreakpoint => "sm";
}