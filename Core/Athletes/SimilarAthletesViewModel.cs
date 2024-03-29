namespace FLRC.Leaderboards.Core.Athletes;

public sealed class SimilarAthletesViewModel : DataTableViewModel
{
	public override string Title => "Similar Athletes";
	public override string ResponsiveBreakpoint => "xl";
	public Athlete Athlete { get; init; }
	public IReadOnlyCollection<SimilarAthlete> Matches { get; init; }
}