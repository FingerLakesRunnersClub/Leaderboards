namespace FLRC.ChallengeDashboard;

public class SimilarAthletesViewModel : DataTableViewModel
{
	public override string Title => "Similar Athletes";
	public override string ResponsiveBreakpoint => "xl";
	public Athlete Athlete { get; init; }
	public IEnumerable<SimilarAthlete> Matches { get; init; }
}