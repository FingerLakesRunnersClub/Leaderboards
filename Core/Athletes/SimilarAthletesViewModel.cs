namespace FLRC.Leaderboards.Core.Athletes;

public sealed class SimilarAthletesViewModel : DataTableViewModel
{
	public override string Title => "Similar Athletes";
	public override string ResponsiveBreakpoint => "xl";
	public AthleteHeader Header { get; init; }
	public Athlete Athlete => Header.Athlete;
	public SimilarAthlete[] Matches { get; init; }
}