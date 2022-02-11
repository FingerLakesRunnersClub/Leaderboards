using FLRC.Leaderboards.Core.Athletes;
using FLRC.Leaderboards.Core.Metrics;

namespace FLRC.Leaderboards.Core.Results;

public class AthleteCourseResultsViewModel : CourseResultsViewModel<Time>
{
	public override string Title => $"{Athlete.Name} — {Course.Race.Name}";
	public Athlete Athlete { get; init; }

	public override string ResponsiveBreakpoint => "sm";
}