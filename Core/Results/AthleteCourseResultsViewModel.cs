using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;

namespace FLRC.Leaderboards.Core.Results;

public sealed class AthleteCourseResultsViewModel : CourseResultsViewModel<Time>
{
	public override string Title => $"{Athlete.Name} — {Course.Name}";
	public Athlete Athlete { get; init; }

	public override string ResponsiveBreakpoint => "sm";
}