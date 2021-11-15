namespace FLRC.ChallengeDashboard;

public class AthleteLogViewModel : DataTableViewModel
{
	public Athlete Athlete { get; init; }
	public RankedList<Time> Results { get; init; }
	public override string Title => $"{Athlete.Name} — Activity Log";
	public override string ResponsiveBreakpoint => "md";
}
