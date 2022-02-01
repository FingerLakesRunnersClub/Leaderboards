namespace FLRC.Leaderboards.Core;

public class AthletesViewModel : DataTableViewModel
{
	public override string Title => "Registered Participants";
	public override string ResponsiveBreakpoint => "md";
	public IDictionary<uint, Athlete> Athletes { get; init; }
}