namespace FLRC.Leaderboards.Core;

public class AthleteCourseResultsViewModel : DataTableViewModel
{
	public override string Title => $"{Athlete.Name} — {Course.Name}";
	public Athlete Athlete { get; init; }
	public Course Course { get; init; }
	public RankedList<Time> Results { get; init; }

	public override string ResponsiveBreakpoint => "sm";
}